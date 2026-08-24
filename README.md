# RuinAR

RuinAR er en Android-først AR-prototype, der placerer en rekonstrueret bygning oven på en ruin og tydeligt skelner mellem dokumenterede, sandsynlige og AI-genererede dele.

Projektets kildekode og assets er beregnet til at ligge i GitHub, så arbejdet kan fortsættes fra enhver computer. Se [cloud-arbejdsgangen](docs/CLOUD_WORKFLOW.md).

## Første prototype

Den nuværende kode opretter automatisk en testscene med:

- AR Foundation til Android/ARCore og senere iOS/ARKit
- placering af en enkel rekonstruktionsmodel ved tryk på en registreret flade
- en demomodel, der også kan placeres foran kameraet uden AR-sporing
- farvekodning af historisk sikkerhed
- GPS-status
- lokal JSON-lagring af ruinens metadata
- en projektstruktur, der kan udvides med rigtige 3D-modeller og en backend

## Åbn projektet

1. Installer Unity Hub på Windows.
2. Installer Unity 6 LTS med Android Build Support, Android SDK/NDK og OpenJDK.
3. Vælg **Open** i Unity Hub og åbn denne mappe.
4. Vent på, at Unity henter pakkerne og kompilerer projektet.
5. Scenen `Assets/RuinAR/Scenes/RuinARPrototype.unity` oprettes automatisk ved første åbning.
6. Åbn **Edit > Project Settings > XR Plug-in Management** og aktivér **ARCore** under Android.
7. Skift platform til Android i Build Profiles og byg til en ARCore-understøttet telefon.

På en senere Mac aktiveres ARKit under iOS i XR Plug-in Management, hvorefter Unity kan generere Xcode-projektet.

## Prototypekontroller

- Peg telefonen mod jorden eller en mur og tryk på en registreret flade.
- Hvis AR-sporing ikke er tilgængelig, vælg **Placér demo foran mig**.
- Brug knapperne til at vise alle dele, kun dokumenterede dele eller AI-fortolkninger.
- Vælg **Nulstil placering** for at prøve igen.

## Projektstruktur

```text
Assets/RuinAR/
  Editor/                Automatisk oprettelse af scene og build-indstillinger
  Scripts/Core/          Data, lokation og offline-lagring
  Scripts/AR/            AR-opsætning, placering og demomodel
Packages/                Unity-pakker
ProjectSettings/         Unity-version
```

## Næste milepæl

1. Vælg en konkret forsøgsruin.
2. Importér en mobiloptimeret 3D-model.
3. Erstat demogeometrien med den rigtige model.
4. Tilføj positionskalibrering mod et eksisterende murhjørne.
5. Feltprøv placering og skala på stedet.
