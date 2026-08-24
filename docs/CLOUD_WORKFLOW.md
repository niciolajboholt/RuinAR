# Cloud-arbejdsgang

Projektet skal kunne fortsættes fra en anden computer uden adgang til den oprindelige maskine.

## Det gemmes i GitHub

- al C#-kode
- Unity-scener og projektindstillinger
- pakkeversioner
- dokumentation
- små billeder og testdata
- referencer til store 3D-, lyd- og videofiler

Mapper som `Library`, `Temp`, `Logs` og lokale builds gemmes ikke. Unity genskaber dem automatisk på en ny computer.

Store binære filer håndteres med Git LFS. Git LFS skal installeres på alle udviklingscomputere, før store assets tilføjes.

## Ny computer

1. Installer GitHub Desktop og log ind.
2. Klon `niciolajboholt/RuinAR`.
3. Installer Unity Hub og Unity-versionen fra `ProjectSettings/ProjectVersion.txt`.
4. Tilføj Android Build Support, Android SDK/NDK og OpenJDK.
5. Åbn den klonede projektmappe i Unity Hub.
6. Vent på, at Unity genskaber `Library` og henter pakker.

## Appens cloud-data

GitHub er kun lager for kildekode og projektassets. Produktionsappen skal senere bruge en særskilt backend til:

- brugerkonti
- ruinregistre og GPS-koordinater
- kilder, licenser og dokumentationsstatus
- versionsstyrede rekonstruktioner
- 3D-modeller, lyd og oversættelser
- behandlingsjobs og slettefrister

Telefonen downloader en signeret offlinepakke for den valgte ruin. Offlinepakken er en cache; den autoritative version ligger i cloud-backenden.

## Hemmeligheder

API-nøgler, Apple-certifikater og adgangstokens må aldrig gemmes direkte i repositoryet. De skal ligge som GitHub Secrets, Unity Cloud-secrets eller i den valgte backend.

