# SpamProtector

## About
Noone likes the spam mails. I was tolerating it but recently one of my mailboxes was attacked by scammers and I'm receiving over 1k spam each day. Configuring filters in my hosting provider was enough till the moment when they removed that functionality. I had to invent something new.

Idea of this suite is very simply. Service is scanning mailbox (only inbox) and checking if the mail is spam or not, based on configured rules (by subject, by domain, by sender). The problem was that this approach is not enough because I lost control and possible important mails. Because of that there are services that are cataloging the spam and removing 
trash after given time.

### Services
- ScanService - service responsible for scanning the mailbox and if spam is found, moving to junk folder
- CatalogService - service responsible for inserting spam mail metadata into database
- MarkingService - service responsible for setting PlannedRemoveTime for each new spam mail
- DeleteService - service responsible for removing given amount of spam mails and for double checking if spam was really removed
- MessageEmailHandlerService - service responsible for listening RabbitMQ messages which hold spam metadata information (insert/update)
- MessageServiceRunHandlerService - service responsible for listening RabbitMQ messages which hold information about service run status

## Architecture
![SpamProtector architecture diagram](https://mczernecki.pl/images_others/SpamProtector.svg)

## Tech Stack
- .NET 5.0
- EntityFrameworkCore
- RabbitMQ
- WorkerService
- MSSQL Server 2019
- MailKit
