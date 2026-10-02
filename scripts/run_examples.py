#!/usr/bin/env python3
"""Run every generated example against a local recording server.

Each example runs as its own process with VERIMOR_BASE_URL pointing at the server.
The server answers 418, so the example exits with an API error after it has sent
its request; the check is that exactly one request arrived with the documented
method and path.

usage: run_examples.py {typescript,python,go,php,dotnet,java} [--root PATH] [--manifest PATH]
"""

from __future__ import annotations

import argparse
import json
import os
import re
import subprocess
import sys
import tempfile
import threading
from http.server import BaseHTTPRequestHandler, ThreadingHTTPServer
from pathlib import Path

MANIFESTS = {
    "php": "examples/operations/manifest.json",
    "dotnet": "examples/Operations/manifest.json",
    "java": "examples/operations.json",
}


class Recorder(BaseHTTPRequestHandler):
    seen: list[tuple[str, str]] = []

    def _record(self) -> None:
        length = int(self.headers.get("Content-Length") or 0)
        if length:
            self.rfile.read(length)
        Recorder.seen.append((self.command, self.path.split("?", 1)[0]))
        body = b'{"detail": "recorded by run_examples"}'
        self.send_response(418)
        self.send_header("Content-Type", "application/json")
        self.send_header("Content-Length", str(len(body)))
        self.end_headers()
        self.wfile.write(body)

    do_GET = do_POST = do_PUT = do_PATCH = do_DELETE = _record

    def log_message(self, *_: object) -> None:
        pass


def _path_pattern(path: str) -> re.Pattern[str]:
    parts = re.split(r"(\{[^}]+\})", path)
    return re.compile(
        "".join("[^/]+" if part.startswith("{") else re.escape(part) for part in parts)
    )


def _commands(language: str, root: Path, entries: list[dict]) -> dict[str, list[str]]:
    """Map each example file to the command that runs it."""
    if language == "go":
        build = Path(tempfile.mkdtemp(prefix="verimor-go-examples-"))
        # One output directory per product: sms and whatsapp both have sendotp.
        for product in sorted({entry["product"] for entry in entries}):
            subprocess.run(
                ["go", "build", "-o", str(build / product) + "/", f"./examples/{product}/..."],
                cwd=root / "packages/go",
                check=True,
            )
        return {
            entry["files"]["go"]: [
                str(build / entry["product"] / Path(entry["files"]["go"]).parent.name)
            ]
            for entry in entries
        }
    if language == "dotnet":
        build = Path(tempfile.mkdtemp(prefix="verimor-dotnet-examples-"))
        subprocess.run(
            ["dotnet", "build", "examples/Examples.csproj", "-c", "Release", "-o", str(build)],
            cwd=root,
            check=True,
            stdout=subprocess.DEVNULL,
        )
        return {
            entry["files"]["dotnet"]: ["dotnet", str(build / "Examples.dll"), entry["key"]]
            for entry in entries
        }
    if language == "java":
        version = re.search(r"^  <version>(.+)</version>$", (root / "pom.xml").read_text(), re.M)
        build = Path(tempfile.mkdtemp(prefix="verimor-java-examples-"))
        maven = ["mvn", "-B", "-q"]
        subprocess.run([*maven, "install", "-DskipTests"], cwd=root, check=True)
        subprocess.run(
            [
                *maven,
                "-f",
                "examples/pom.xml",
                f"-Dverimor.version={version.group(1)}",
                "compile",
                "dependency:build-classpath",
                f"-Dmdep.outputFile={build / 'classpath'}",
            ],
            cwd=root,
            check=True,
        )
        dependencies = (build / "classpath").read_text()
        classpath = f"{root / 'examples/target/classes'}{os.pathsep}{dependencies}"
        return {
            entry["files"]["java"]: ["java", "-cp", classpath, entry["key"]] for entry in entries
        }
    runner = {"typescript": ["node"], "python": [sys.executable], "php": ["php"]}[language]
    return {
        entry["files"][language]: [*runner, str(root / entry["files"][language])]
        for entry in entries
    }


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("language", choices=["typescript", "python", "go", "php", "dotnet", "java"])
    parser.add_argument("--root", type=Path, default=Path(__file__).resolve().parents[1])
    parser.add_argument("--manifest")
    arguments = parser.parse_args()
    root = arguments.root.resolve()
    manifest = arguments.manifest or MANIFESTS.get(arguments.language, "examples/manifest.json")
    entries = json.loads((root / manifest).read_text())

    server = ThreadingHTTPServer(("127.0.0.1", 0), Recorder)
    threading.Thread(target=server.serve_forever, daemon=True).start()
    env = {
        **os.environ,
        "VERIMOR_BASE_URL": f"http://127.0.0.1:{server.server_port}",
        "VERIMOR_SMS_USERNAME": "example-user",
        "VERIMOR_SMS_PASSWORD": "example-password",
        "VERIMOR_SWITCH_API_KEY": "example-switch-key",
        "VERIMOR_WHATSAPP_API_KEY": "example-whatsapp-key",
    }
    commands = _commands(arguments.language, root, entries)
    failures = []
    for entry in entries:
        file = entry["files"][arguments.language]
        Recorder.seen = []
        completed = subprocess.run(
            commands[file], cwd=root, env=env, capture_output=True, text=True, timeout=120
        )
        seen = list(Recorder.seen)
        expected = (entry["method"], _path_pattern(entry["path"]))
        if len(seen) != 1 or seen[0][0] != expected[0] or not expected[1].fullmatch(seen[0][1]):
            failures.append(f"{file}: sent {seen}\n{completed.stdout}{completed.stderr}".strip())
    server.shutdown()
    for failure in failures:
        print(f"FAIL {failure}\n", file=sys.stderr)
    passed = len(entries) - len(failures)
    print(f"{arguments.language}: {passed}/{len(entries)} examples sent the documented request")
    return 1 if failures else 0


if __name__ == "__main__":
    raise SystemExit(main())
