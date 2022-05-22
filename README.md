# GitHub Bookmark Generator

Generates a 2D SVG of your github activity over a set period that you can use to create a custom bookmark for.

Intended for use with a cricket cutter OR laser cutter.

You can also extrude this svg to create a 3D printable version.

![Sample of the Generated Bookmark outline](sample-bookmark.svg)

## Running This:

Get a GitHub Personal Access Token

Run BookmarkGenConsole with the Access Token and Username you want to generate the bookmark for:

```sh
dotnet run -- [TOKEN] [USERNAME]
```

Use `dotnet run -- --help` to get help and learn the other available options.

## Usage

```
Usage:
  BookmarkGenConsole <--token> <--username> [options]

Arguments:
  <--token>     Your GitHub Personal Access Token
  <--username>  The username whose contributions should be graphed

Options:
  --width <width>                  The width in mm of the bookmark to generate [default: 200]
  --height <height>                The height in mm of the bookmark to generate [default: 40]
  --min-height <min-height>        The minimum height of the lowest contribution bar [default: 20]
  --corner-radius <corner-radius>  The corner radius to use in the generated bookmark. Must be greater than or equal to half the min-height.
                                   [default: 10]
  --output <output>                The output file path for the contributions bookmark SVG [default: contributions.svg]
  --from <from>                    The date to begin collecting contributions from [default: 1 year ago]
  --to <to>                        The date to end collecting contributions from [default: the current day]
  --label <label>                  The label to display on the right hand side of the bookmark []
  --version                        Show version information
  -?, -h, --help                   Show help and usage information
  ```