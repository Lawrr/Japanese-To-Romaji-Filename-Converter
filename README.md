# Japanese to Romaji Filename Converter
![](https://puu.sh/xcvkz/01b4cef68b.png)

Convert filenames with japanese characters (hiragana/kanji/katakana) into romaji/english - with the help of Google Translate.

Hiragana and kanji characters are converted into romaji, while katakana is converted into the english words.

# [Download Here](https://github.com/Lawrr/Japanese-To-Romaji-Filename-Converter/releases)
## Requirements
- .NET Framework 4.5

# Current Features
- **Convert filenames**: from japanese to romaji/english.
- **Revert conversions**: ability to change filenames back after a conversion.
- **Phrase mapping**: helps to correct incorrect translations.
- **Particle list**: choose which words you want capitalised.
- **Audio metadata support**: if the file is an audio file with ID3 metadata, this program also converts the title, artists, album artist and album information into romaji/english if it is in japanese.

# Program Usage

### Getting Started
1. Extract/unzip the downloaded zip file (**Please unzip before using it so that the conversion-history file can be written to properly**)
2. Run `Japanese to Romaji Filename Converter.exe`

### Converting
1. Add some files to be converted
2. Click `Convert`

### Reverting
1. Click `View Conversion History`
2. Select files which you want converted back
3. Click `Revert Selected`

# Things To Know When Using The Program
## Conversion History
Whenever a conversion occurs, the conversion is saved in the `conversion-history.xml` file in the same directory as the program. The conversion data is saved in this file in case a file has to be reverted back.

## Mappings
To ease the problem of incorrect translations, there are mapping files located in `res/maps/` which will map the specified phrases into another phrase. For example, if the phrase `tsu` continuously gets translated into `tsud`, you can create a new mapping `tsud:tsu` which will map `tsud` to `tsu` whenever a translation occurs.

**Mappings support regular expressions.**

Currently there are two mapping files:
- `hirakanji-latn_maps.txt`: Used whenever a _token_ is translated from hiragana/kanji to romaji
- `kata-en_maps.txt`: Used whenever a _token_ is translated from katakana to english

## Particles
`res/particles/` contains a list of language particles which do not get capitalised during conversion.

**Particles support regular expressions.**

Currently there are two particle files:
- `hirakanji-latn_particles.txt`: list of japanese particles (romanized) which are checked when a _token_ is translated from hiragana/kanji to romaji
- `kata-en_particles.txt`: list of english particles which are checked when a _token_ is translated from katakana to english

# Development Dependencies
- HtmlAgilityPack
- TagLib

# Disclaimer
Please be aware that the software is not perfect and may make incorrect translations. Please use at your own risk.
