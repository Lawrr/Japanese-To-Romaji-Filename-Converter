# Japanese to Romaji Filename Converter
Works for any file with a japanese name (hiragana/kanji/katakana). Converts the filename into romaji with the help of Google Translator.

Note: This overrides the selected files with the new converted filenames.

#### Metadata
For audio files it also converts the metadata tags for:
- Title
- Contributing artists
- Album artists
- Album

## Current Features
- Convert files from japanese to romaji (including metadata)
- Revert files (and metadata) back to their original names after a conversion
- Extendable mapping list for incorrect translations
- Extendable particle list for non-capitalised phrases

## Usage
Note: Please unzip the folder before using it so that the conversion-history file can be written to permanently.

#### Getting Started
1. Extract the zip file
2. Run `Japanese to Romaji Filename Converter.exe`

#### Converting
1. Add some files to be converted
2. Click `Convert`

#### Reverting
1. Click `View Conversion History`
2. Select files which you want converted back
3. Click `Revert Selected`

## Things To Know
### Conversion History
Whenever a conversion occurs, the conversion is saved in the `conversion-history.xml` file in the same directory as the program. The conversion data is saved in this file in case a file has to be reverted back.

### Mappings
To ease the problem of incorrect translations, there are mapping files located in `res/maps/` which will map the specified phrases into another phrase. For example, if the phrase `tsu` continuously gets translated into `tsud`, you can create a new mapping `tsud:tsu` which will map `tsud` to `tsu` whenever a translation occurs.

Currently there are two mapping files:
- `hirakanji-latn_maps.txt`: Used whenever a token is translated from hiragana/kanji to romaji
- `kata-latn_maps.txt`: Used whenever a token is translated from katakana to english

### Particles
`res/particles/` contains a list of language particles which do not get capitalised during conversion.

Currently there are two particle files:
- `hirakanji-latn_particles.txt`: list of japanese particles (romanized) which are checked when a token is translated from hiragana/kanji to romaji
- `kata-latn_particles.txt`: list of english particles which are checked when a token is translated from katakana to english

## Runtime Dependencies
- .NET Framework 4.5

## Development Dependencies
- HtmlAgilityPack
- TagLib

## Disclaimer
Please be aware that the software is not perfect and may make incorrect translations. Please use at your own risk.
