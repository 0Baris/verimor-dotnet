# Verimor .NET SDK — reference for AI assistants

Unofficial .NET client targeting netstandard2.0 (.NET Framework 4.6.2+, .NET 8, .NET 10) for Verimor SMS, Switch and WhatsApp.

```bash
dotnet add package BarisCemant.Verimor
```

## Setting the server

```csharp
var sms = new SmsClient(new SmsClientOptions
{
    Username = username,
    Password = password,
    DefaultSender = "VERIMOR",
    BaseUri = new Uri("https://sms.example.test"),
});
```

`SwitchClientOptions { Key }` and `WhatsAppClientOptions { ApiKey }` take `BaseUri` the same way;
leave it out to use Verimor's server.

## Calling operations

Each client groups operations into services: `await client.{Service}.{Operation}Async(...)`.
Request models live in `BarisCemant.Verimor.{Sms|Switch|WhatsApp}.Generated.Model`; pass
`""` for `username`/`password` in SMS request models, the client fills them in.

## Errors

A non-2xx answer throws `VerimorApiException` with the status code and body;
`UnexpectedResponseException` means a 2xx body did not match the documented shape.

Run one example with `dotnet run --project examples -- sms/send`.

## Products and authentication

| Product | Credentials | Default server | Environment variables used by the examples |
| --- | --- | --- | --- |
| SMS | username + password, optional default sender (`source_addr`) | `https://sms.verimor.com.tr` | `VERIMOR_SMS_USERNAME`, `VERIMOR_SMS_PASSWORD`, `VERIMOR_SMS_SENDER` |
| Switch | API key (`key`) | `https://api.bulutsantralim.com` | `VERIMOR_SWITCH_API_KEY` |
| WhatsApp | API key (`x-api-key` header) | `https://wapi.verimor.com.tr` | `VERIMOR_WHATSAPP_API_KEY` |

Every client talks to Verimor's server by default. Pass a different base URL to use a proxy,
a test server or a mock; every example reads it from `VERIMOR_BASE_URL`. The client adds the
credentials to each request itself (query, body or header, as the operation requires), so
request values never carry them. Keep credentials on the server side.

The SMS default sender is sent as `source_addr` wherever an operation accepts one and the
call does not set it.

## Running an example

Every operation has a runnable example. Set the environment variables above and run the
file; set `VERIMOR_BASE_URL` to point it at your own server. The repository's
`scripts/run_examples.py` runs all of them against a local recording server, which never
contacts Verimor.

## Operations

Each operation: HTTP method and path, the call, and the runnable example file. Values are samples from the API documentation.

### SMS

#### addBlacklistEntry — `POST /v2/blacklists`

Kara Liste Ekleme

C# — [`examples/Operations/Sms/AddBlacklistEntry.cs`](examples/Operations/Sms/AddBlacklistEntry.cs)

```csharp
client.Blacklist.AddBlacklistEntryAsync(
    phones: "905001112233"
)
```


#### balance — `GET /v2/balance`

Bakiye Sorgulama

C# — [`examples/Operations/Sms/Balance.cs`](examples/Operations/Sms/Balance.cs)

```csharp
client.Balances.BalanceAsync()
```


#### cancel — `POST /v2/cancel/{id}`

Gönderim İptali

C# — [`examples/Operations/Sms/Cancel.cs`](examples/Operations/Sms/Cancel.cs)

```csharp
client.Campaigns.CancelAsync(
    id: "123",
    request: new PostV2CancelIdRequest(
        username: "",
        password: "")
)
```


#### deleteBlacklistEntry — `DELETE /v2/blacklists/{id}`

Kara Listeden Silme

C# — [`examples/Operations/Sms/DeleteBlacklistEntry.cs`](examples/Operations/Sms/DeleteBlacklistEntry.cs)

```csharp
client.Blacklist.DeleteBlacklistEntryAsync(
    id: "123"
)
```


#### listBlacklistEntries — `GET /v2/blacklists`

Kara Liste Görüntüleme

C# — [`examples/Operations/Sms/ListBlacklistEntries.cs`](examples/Operations/Sms/ListBlacklistEntries.cs)

```csharp
client.Blacklist.ListBlacklistEntriesAsync()
```


#### listInboundMessages — `GET /v2/inbound_messages`

Gelen SMS Sorgulama

C# — [`examples/Operations/Sms/ListInboundMessages.cs`](examples/Operations/Sms/ListInboundMessages.cs)

```csharp
client.Reports.ListInboundMessagesAsync()
```


#### listIysCampaignConsents — `GET /v2/iys/campaigns/{id}/consents`

İYS İzinleri Sorgulama

C# — [`examples/Operations/Sms/ListIysCampaignConsents.cs`](examples/Operations/Sms/ListIysCampaignConsents.cs)

```csharp
client.Iys.ListIysCampaignConsentsAsync(
    id: 1
)
```


#### listIysCampaigns — `GET /v2/iys/campaigns`

İYS Kampanyaları Listeleme

C# — [`examples/Operations/Sms/ListIysCampaigns.cs`](examples/Operations/Sms/ListIysCampaigns.cs)

```csharp
client.Iys.ListIysCampaignsAsync()
```


#### listSenderIds — `GET /v2/headers`

Başlık Yönetimi

C# — [`examples/Operations/Sms/ListSenderIds.cs`](examples/Operations/Sms/ListSenderIds.cs)

```csharp
client.SenderIds.ListSenderIdsAsync()
```


#### send — `POST /v2/send.json`

SMS Gönderme (JSON)

C# — [`examples/Operations/Sms/Send.cs`](examples/Operations/Sms/Send.cs)

```csharp
client.Campaigns.SendAsync(
    request: new SendSmsJsonRequest(
        username: "",
        password: "",
        messages: new List<SendSmsJsonRequestMessagesInner>
        {
            new SendSmsJsonRequestMessagesInner(
                dest: "905111111111,905111111112",
                msg: "Deneme Mesaj"),
        })
)
```


#### sendLegacy — `GET /v2/send`

SMS Gönderme (GET)

C# — [`examples/Operations/Sms/SendLegacy.cs`](examples/Operations/Sms/SendLegacy.cs)

```csharp
client.Campaigns.SendLegacyAsync(
    dest: "905001112233",
    msg: "Merhaba"
)
```


#### sendOtp — `POST /v2/otp`

OTP Gönderme

C# — [`examples/Operations/Sms/SendOtp.cs`](examples/Operations/Sms/SendOtp.cs)

```csharp
client.Campaigns.SendOtpAsync(
    request: new OtpRequest(
        username: "",
        password: "",
        dest: "905001234567",
        code: "482931")
)
```


#### status — `GET /v2/status`

Rapor Sorgulama (API ID)

C# — [`examples/Operations/Sms/Status.cs`](examples/Operations/Sms/Status.cs)

```csharp
client.Reports.StatusAsync(
    id: 1
)
```


#### submitIysConsents — `POST /v2/iys_consents.json`

İzin Yönetimi

C# — [`examples/Operations/Sms/SubmitIysConsents.cs`](examples/Operations/Sms/SubmitIysConsents.cs)

```csharp
client.Iys.SubmitIysConsentsAsync(
    request: new PostV2IysConsentsJsonRequest(
        username: "",
        password: "",
        sourceAddr: Environment.GetEnvironmentVariable("VERIMOR_SMS_SENDER") ?? "VERIMOR",
        consents: new List<PostV2IysConsentsJsonRequestConsentsInner>
        {
            new PostV2IysConsentsJsonRequestConsentsInner(
                type: "MESAJ",
                source: "HS_WEB",
                status: "ONAY",
                recipientType: "BIREYSEL",
                consentDate: DateTimeOffset.Parse("2022-04-14 13:30:30", CultureInfo.InvariantCulture),
                recipient: "905001112233"),
        })
)
```


### Switch

#### answer — `POST /answer`

Çağrıyı Cevaplama (POST)

C# — [`examples/Operations/Switch/Answer.cs`](examples/Operations/Switch/Answer.cs)

```csharp
client.Calls.AnswerAsync(
    request: new AnswerCallPostRequest(
        id: "736eaf7e-4cc4-44ab-8dbe-16b18e9618b1")
)
```


#### answerLegacy — `GET /answer/{id}`

Çağrıyı Cevaplama (GET)

C# — [`examples/Operations/Switch/AnswerLegacy.cs`](examples/Operations/Switch/AnswerLegacy.cs)

```csharp
client.Calls.AnswerLegacyAsync(
    id: "736eaf7e-4cc4-44ab-8dbe-16b18e9618b1"
)
```


#### bridge — `GET /bridge`

Çağrı Bağlama

C# — [`examples/Operations/Switch/Bridge.cs`](examples/Operations/Switch/Bridge.cs)

```csharp
client.Calls.BridgeAsync(
    source: "905111111111",
    destination: "905111111112"
)
```


#### createAnnouncement — `POST /announcements`

Yeni Ses Dosyası Yükleme

C# — [`examples/Operations/Switch/CreateAnnouncement.cs`](examples/Operations/Switch/CreateAnnouncement.cs)

```csharp
client.Announcements.CreateAnnouncementAsync(
    name: "dosya adı",
    sounddata: "base64"
)
```


#### createBlockedNumber — `POST /blocked_numbers`

Kara Listeye Ekleme

C# — [`examples/Operations/Switch/CreateBlockedNumber.cs`](examples/Operations/Switch/CreateBlockedNumber.cs)

```csharp
client.Blacklist.CreateBlockedNumberAsync(
    number: "05111111111"
)
```


#### createContact — `POST /contacts`

Kişi Ekleme

C# — [`examples/Operations/Switch/CreateContact.cs`](examples/Operations/Switch/CreateContact.cs)

```csharp
client.Contacts.CreateContactAsync(
    name: "Verimor",
    surname: "Telekomünikasyon",
    phone: "05111111111"
)
```


#### createContactGroup — `POST /contact_groups`

Grup Oluşturma

C# — [`examples/Operations/Switch/CreateContactGroup.cs`](examples/Operations/Switch/CreateContactGroup.cs)

```csharp
client.Contacts.CreateContactGroupAsync(
    name: "Müşteriler"
)
```


#### createFaxDocumentUrl — `POST /fax_document_url`

Faks Belgesi URL'si İsteme

C# — [`examples/Operations/Switch/CreateFaxDocumentUrl.cs`](examples/Operations/Switch/CreateFaxDocumentUrl.cs)

```csharp
client.Fax.CreateFaxDocumentUrlAsync(
    callUuid: "e28e5d48-05d8-11e8-663a-fde60c59425c"
)
```


#### createFaxOrder — `POST /fax_orders`

Faks Gönderimi

C# — [`examples/Operations/Switch/CreateFaxOrder.cs`](examples/Operations/Switch/CreateFaxOrder.cs)

```csharp
client.Fax.CreateFaxOrderAsync(
    remoteStationId: "901234567891",
    filedata: "JVBERi0xLjQK"
)
```


#### createIvrCampaign — `POST /ivr_campaigns.json`

Otomatik Arama Kampanyası Oluşturma

C# — [`examples/Operations/Switch/CreateIvrCampaign.cs`](examples/Operations/Switch/CreateIvrCampaign.cs)

```csharp
client.IvrCampaigns.CreateIvrCampaignAsync(
    request: new CreateIvrCampaignRequest(
        callType: "ivr",
        name: "Memnuniyet anketi",
        phoneList: new List<CreateIvrCampaignRequestPhoneListInner>
        {
            new CreateIvrCampaignRequestPhoneListInner(
                phone: "05111111111",
                phrase: "#429 12/05/2017 #430 102.45 #431",
                lang: "tr-TR"),
            new CreateIvrCampaignRequestPhoneListInner(
                phone: "05111111112",
                phrase: "#429 12/05/2017 #430 65.12 #431",
                lang: "tr-TR"),
        })
)
```


#### createRecordingUrl — `POST /recording_url`

Ses Kaydı için Geçici URL Oluşturma

C# — [`examples/Operations/Switch/CreateRecordingUrl.cs`](examples/Operations/Switch/CreateRecordingUrl.cs)

```csharp
client.Records.CreateRecordingUrlAsync(
    callUuid: "3f2504e0-4f89-41d3-9a0c-0305e82c3301"
)
```


#### createVoicemailRecordingUrl — `POST /voicemail_recording_url`

Telesekreter Ses Kaydı için Geçici URL Oluşturma

C# — [`examples/Operations/Switch/CreateVoicemailRecordingUrl.cs`](examples/Operations/Switch/CreateVoicemailRecordingUrl.cs)

```csharp
client.Records.CreateVoicemailRecordingUrlAsync(
    uuid: "12345678-1234-5678-4321-123456789012"
)
```


#### createWebphoneToken — `POST /webphone_tokens`

Dahili için Token Alma (IFrame ile kullanmak için)

C# — [`examples/Operations/Switch/CreateWebphoneToken.cs`](examples/Operations/Switch/CreateWebphoneToken.cs)

```csharp
client.Users.CreateWebphoneTokenAsync(
    extension: "1001"
)
```


#### deleteAnnouncement — `DELETE /announcements/{id}`

Ses Dosyası Silme

C# — [`examples/Operations/Switch/DeleteAnnouncement.cs`](examples/Operations/Switch/DeleteAnnouncement.cs)

```csharp
client.Announcements.DeleteAnnouncementAsync(
    id: "123"
)
```


#### deleteBlockedNumber — `DELETE /blocked_numbers/delete`

Kara Listeden Silme

C# — [`examples/Operations/Switch/DeleteBlockedNumber.cs`](examples/Operations/Switch/DeleteBlockedNumber.cs)

```csharp
client.Blacklist.DeleteBlockedNumberAsync(
    number: "05111111111"
)
```


#### deleteContact — `DELETE /contacts/{id}`

Kişi Silme

C# — [`examples/Operations/Switch/DeleteContact.cs`](examples/Operations/Switch/DeleteContact.cs)

```csharp
client.Contacts.DeleteContactAsync(
    id: 1
)
```


#### deleteContactGroup — `DELETE /contact_groups/{id}`

Grup Silme

C# — [`examples/Operations/Switch/DeleteContactGroup.cs`](examples/Operations/Switch/DeleteContactGroup.cs)

```csharp
client.Contacts.DeleteContactGroupAsync(
    id: 1
)
```


#### deleteIvrCampaign — `DELETE /ivr_campaigns/{id}.json`

Otomatik Arama Kampanyasını Silme

C# — [`examples/Operations/Switch/DeleteIvrCampaign.cs`](examples/Operations/Switch/DeleteIvrCampaign.cs)

```csharp
client.IvrCampaigns.DeleteIvrCampaignAsync(
    id: "123"
)
```


#### downloadFaxDocument — `GET /fax_document/{id}`

Faks Belgesi İndirme/Görüntüleme

C# — [`examples/Operations/Switch/DownloadFaxDocument.cs`](examples/Operations/Switch/DownloadFaxDocument.cs)

```csharp
client.Fax.DownloadFaxDocumentAsync(
    id: "123"
)
```


#### getCdr — `GET /cdrs/{id}`

Belirli Bir Çağrının Detaylı CDR Kaydı

C# — [`examples/Operations/Switch/GetCdr.cs`](examples/Operations/Switch/GetCdr.cs)

```csharp
client.Records.GetCdrAsync(
    id: "call-uuid-12345-67890"
)
```


#### getCrmIntegrations — `GET /crm_integrations`

CRM Entegrasyon Ayarlarını Getirme

C# — [`examples/Operations/Switch/GetCrmIntegrations.cs`](examples/Operations/Switch/GetCrmIntegrations.cs)

```csharp
client.Crm.GetCrmIntegrationsAsync()
```


#### getExtension — `GET /extensions/{id}`

Dahili Detayı

C# — [`examples/Operations/Switch/GetExtension.cs`](examples/Operations/Switch/GetExtension.cs)

```csharp
client.Users.GetExtensionAsync(
    id: "1001"
)
```


#### getWebhookPayloadExamples — `GET /webhook-payload-examples`

CRM Webhook Payload Örnekleri

C# — [`examples/Operations/Switch/GetWebhookPayloadExamples.cs`](examples/Operations/Switch/GetWebhookPayloadExamples.cs)

```csharp
client.Crm.GetWebhookPayloadExamplesAsync()
```


#### hangup — `GET /hangup/{id}`

Çağrıyı Sonlandırma

C# — [`examples/Operations/Switch/Hangup.cs`](examples/Operations/Switch/Hangup.cs)

```csharp
client.Calls.HangupAsync(
    id: "f3797dfc-a818-11e7-bf70-cb295b6663ce"
)
```


#### listAgentStatuses — `GET /agent_statuses`

MT Durumlarını ve Üyeliklerini Listeleme

C# — [`examples/Operations/Switch/ListAgentStatuses.cs`](examples/Operations/Switch/ListAgentStatuses.cs)

```csharp
client.Users.ListAgentStatusesAsync()
```


#### listAnnouncements — `GET /announcements`

Ses Dosyaları Listesine Erişim

C# — [`examples/Operations/Switch/ListAnnouncements.cs`](examples/Operations/Switch/ListAnnouncements.cs)

```csharp
client.Announcements.ListAnnouncementsAsync()
```


#### listBlockedNumbers — `GET /blocked_numbers`

Kara Listeye Erişim

C# — [`examples/Operations/Switch/ListBlockedNumbers.cs`](examples/Operations/Switch/ListBlockedNumbers.cs)

```csharp
client.Blacklist.ListBlockedNumbersAsync()
```


#### listCallerIds — `GET /caller_ids`

Dış Numaralar Listesine Erişim

C# — [`examples/Operations/Switch/ListCallerIds.cs`](examples/Operations/Switch/ListCallerIds.cs)

```csharp
client.CallerIds.ListCallerIdsAsync()
```


#### listCdrs — `GET /cdrs`

Çağrı Detay Kayıtları (CDR) Listesi

C# — [`examples/Operations/Switch/ListCdrs.cs`](examples/Operations/Switch/ListCdrs.cs)

```csharp
client.Records.ListCdrsAsync()
```


#### listContactGroups — `GET /contact_groups`

Grup Listesine Erişim

C# — [`examples/Operations/Switch/ListContactGroups.cs`](examples/Operations/Switch/ListContactGroups.cs)

```csharp
client.Contacts.ListContactGroupsAsync()
```


#### listContacts — `GET /contacts`

Kişiler Listesine Erişim

C# — [`examples/Operations/Switch/ListContacts.cs`](examples/Operations/Switch/ListContacts.cs)

```csharp
client.Contacts.ListContactsAsync()
```


#### listExtensions — `GET /extensions`

Dahili Listesi

C# — [`examples/Operations/Switch/ListExtensions.cs`](examples/Operations/Switch/ListExtensions.cs)

```csharp
client.Users.ListExtensionsAsync()
```


#### listFaxOrders — `GET /fax_orders`

Tamamlanmamış Faks Gönderimlerinin Listesi

C# — [`examples/Operations/Switch/ListFaxOrders.cs`](examples/Operations/Switch/ListFaxOrders.cs)

```csharp
client.Fax.ListFaxOrdersAsync()
```


#### listFaxRecords — `GET /fdrs`

Faks Listesine Erişim

C# — [`examples/Operations/Switch/ListFaxRecords.cs`](examples/Operations/Switch/ListFaxRecords.cs)

```csharp
client.Fax.ListFaxRecordsAsync()
```


#### listQueuePendingCalls — `GET /queues/pending`

Kuyrukta Bekleyenler Listesine Erişim

C# — [`examples/Operations/Switch/ListQueuePendingCalls.cs`](examples/Operations/Switch/ListQueuePendingCalls.cs)

```csharp
client.Queues.ListQueuePendingCallsAsync()
```


#### listQueueUsers — `GET /queue/user_list`

Kuyruktaki Dahili Listesine Erişim

C# — [`examples/Operations/Switch/ListQueueUsers.cs`](examples/Operations/Switch/ListQueueUsers.cs)

```csharp
client.Queues.ListQueueUsersAsync(
    queueNumber: "200"
)
```


#### listQueues — `GET /queues`

Kuyruklar Listesine Erişim

C# — [`examples/Operations/Switch/ListQueues.cs`](examples/Operations/Switch/ListQueues.cs)

```csharp
client.Queues.ListQueuesAsync()
```


#### listUserStatuses — `GET /user_statuses`

Dahili Durumlarını Listeleme

C# — [`examples/Operations/Switch/ListUserStatuses.cs`](examples/Operations/Switch/ListUserStatuses.cs)

```csharp
client.Users.ListUserStatusesAsync()
```


#### listVoicemailMessages — `GET /voicemail_messages`

Telesekreter Arama Kayıtlarına Erişim

C# — [`examples/Operations/Switch/ListVoicemailMessages.cs`](examples/Operations/Switch/ListVoicemailMessages.cs)

```csharp
client.Records.ListVoicemailMessagesAsync()
```


#### manageQueueUsers — `GET /queue/manage_users`

Kuyruğa Dahili Ekleme, Çıkarma veya Yer Değiştirme

C# — [`examples/Operations/Switch/ManageQueueUsers.cs`](examples/Operations/Switch/ManageQueueUsers.cs)

```csharp
client.Queues.ManageQueueUsersAsync(
    queueNumber: "200",
    userList: "1000,1001,1002"
)
```


#### originate — `POST /originate`

Çağrı Başlatma (POST)

C# — [`examples/Operations/Switch/Originate.cs`](examples/Operations/Switch/Originate.cs)

```csharp
client.Calls.OriginateAsync(
    request: new OriginateCallPostRequest(
        extension: "1001",
        destination: "908505320000")
)
```


#### originateLegacy — `GET /originate`

Çağrı Başlatma (GET)

C# — [`examples/Operations/Switch/OriginateLegacy.cs`](examples/Operations/Switch/OriginateLegacy.cs)

```csharp
client.Calls.OriginateLegacyAsync(
    extension: "1001",
    destination: "908505320000"
)
```


#### setCallMute — `GET /mute/{id}`

Çağrıyı Sessize Alma / Sesli Yapma

C# — [`examples/Operations/Switch/SetCallMute.cs`](examples/Operations/Switch/SetCallMute.cs)

```csharp
client.Calls.SetCallMuteAsync(
    id: "f3797dfc-a818-11e7-bf70-cb295b6663ce",
    state: "on"
)
```


#### setDnd — `GET /dnd/{id}`

Dahili için Rahatsız Etme (DND) Modunu Ayarlama

C# — [`examples/Operations/Switch/SetDnd.cs`](examples/Operations/Switch/SetDnd.cs)

```csharp
client.Users.SetDndAsync(
    id: "1001",
    state: "on"
)
```


#### transfer — `POST /transfer`

Çağrıyı Aktarma (POST)

C# — [`examples/Operations/Switch/Transfer.cs`](examples/Operations/Switch/Transfer.cs)

```csharp
client.Calls.TransferAsync(
    id: "f3797dfc-a818-11e7-bf70-cb295b6663ce",
    userNumber: "1000"
)
```


#### transferLegacy — `GET /transfer/{id}`

Çağrıyı Aktarma (GET)

C# — [`examples/Operations/Switch/TransferLegacy.cs`](examples/Operations/Switch/TransferLegacy.cs)

```csharp
client.Calls.TransferLegacyAsync(
    id: "f3797dfc-a818-11e7-bf70-cb295b6663ce",
    userNumber: "1000"
)
```


#### updateAnnouncement — `PATCH /announcements/{id}`

Ses Dosyası Güncelleme

C# — [`examples/Operations/Switch/UpdateAnnouncement.cs`](examples/Operations/Switch/UpdateAnnouncement.cs)

```csharp
client.Announcements.UpdateAnnouncementAsync(
    id: "123"
)
```


#### updateContact — `PATCH /contacts/{id}`

Kişi Güncelleme

C# — [`examples/Operations/Switch/UpdateContact.cs`](examples/Operations/Switch/UpdateContact.cs)

```csharp
client.Contacts.UpdateContactAsync(
    id: 1
)
```


#### updateContactGroup — `PATCH /contact_groups/{id}`

Grup Güncelleme

C# — [`examples/Operations/Switch/UpdateContactGroup.cs`](examples/Operations/Switch/UpdateContactGroup.cs)

```csharp
client.Contacts.UpdateContactGroupAsync(
    id: 1,
    name: "Arkadaşlarım"
)
```


#### updateCrmIntegrations — `POST /crm_integrations`

CRM Entegrasyon Ayarlarını Güncelleme

C# — [`examples/Operations/Switch/UpdateCrmIntegrations.cs`](examples/Operations/Switch/UpdateCrmIntegrations.cs)

```csharp
client.Crm.UpdateCrmIntegrationsAsync()
```


#### updateIvrCampaign — `PATCH /ivr_campaigns/{id}.json`

Otomatik Arama Kampanyasını Başlatma/Durdurma

C# — [`examples/Operations/Switch/UpdateIvrCampaign.cs`](examples/Operations/Switch/UpdateIvrCampaign.cs)

```csharp
client.IvrCampaigns.UpdateIvrCampaignAsync(
    id: "123",
    status: "on"
)
```


#### updateOutboundCallerId — `GET /update_outbound_caller_id`

Dahilinin Dış Numarasını (Arayan No) Değiştirme

C# — [`examples/Operations/Switch/UpdateOutboundCallerId.cs`](examples/Operations/Switch/UpdateOutboundCallerId.cs)

```csharp
client.CallerIds.UpdateOutboundCallerIdAsync(
    extension: "1000",
    callerId: "90850532xxxx"
)
```


### WhatsApp

#### getMessage — `GET /v1/messages/{message_ref}`

Mesaj Kaydını Sorgula

C# — [`examples/Operations/WhatsApp/GetMessage.cs`](examples/Operations/WhatsApp/GetMessage.cs)

```csharp
client.Messages.GetMessageAsync(
    messageRef: "3f2504e0-4f89-41d3-9a0c-0305e82c3301"
)
```


#### health — `GET /health`

Health check

C# — [`examples/Operations/WhatsApp/Health.cs`](examples/Operations/WhatsApp/Health.cs)

```csharp
client.Health.HealthAsync()
```


#### listMessages — `GET /v1/messages`

Mesajları Listele / Ara

C# — [`examples/Operations/WhatsApp/ListMessages.cs`](examples/Operations/WhatsApp/ListMessages.cs)

```csharp
client.Messages.ListMessagesAsync()
```


#### sendBulk — `POST /v1/messages/bulk`

Toplu Şablon Mesajı Gönder

C# — [`examples/Operations/WhatsApp/SendBulk.cs`](examples/Operations/WhatsApp/SendBulk.cs)

```csharp
client.Messages.SendBulkAsync(
    request: new BulkMessageRequest(
        templateName: "odeme_hatirlatici",
        recipients: new List<BulkRecipient>
        {
            new BulkRecipient(
                to: "905001112233"),
        })
)
```


#### sendOtp — `POST /v1/messages/otp`

OTP / Kimlik Doğrulama Mesajı Gönder

C# — [`examples/Operations/WhatsApp/SendOtp.cs`](examples/Operations/WhatsApp/SendOtp.cs)

```csharp
client.Messages.SendOtpAsync(
    request: new TemplateMessageRequest(
        to: "905001112233",
        templateName: "siparis_onay")
)
```


#### sendUtility — `POST /v1/messages/utility`

Utility / İşlemsel Mesaj Gönder

C# — [`examples/Operations/WhatsApp/SendUtility.cs`](examples/Operations/WhatsApp/SendUtility.cs)

```csharp
client.Messages.SendUtilityAsync(
    request: new TemplateMessageRequest(
        to: "905001112233",
        templateName: "siparis_onay")
)
```
