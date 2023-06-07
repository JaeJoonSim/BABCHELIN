# Bitmap Font Importer

[![Test](https://github.com/litefeel/Unity-BitmapFontImporter/workflows/Test/badge.svg)](https://github.com/litefeel/Unity-BitmapFontImporter/actions)
[![](https://img.shields.io/github/release/litefeel/Unity-BitmapFontImporter.svg?label=latest%20version)](https://github.com/litefeel/Unity-BitmapFontImporter/releases)
[![](https://img.shields.io/github/license/litefeel/Unity-BitmapFontImporter.svg)](https://github.com/litefeel/Unity-BitmapFontImporter/blob/upm/LICENSE.md)
[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://paypal.me/litefeel)

[Bitmap Font Importer][bfi] is just perfect Unity asset plugin to import any custom bitmap font to your project.  
It easy imports any bitmap font generated by third party tools like:[littera][1], [bmGlyph][2], [Glyph Designer 2][3], [ShoeBox][4] or [Bitmap Font Generator][5]

Thiks @Xylph, this origin code form [here](http://forum.unity3d.com/threads/unity-4-6-bitmap-font.265209/)

## Feature

- Free
- Auto Import bitmap fonts
- Support XML and Plain text format of .fnt
- Support Multi texture
- Reimport exists bitmap font
- Source Code on [Github][bfi]
- Tested with Unity 2019.4 and 2020.3
- No runtime resources required
- No scripting required


## Install

#### Using npm (Ease upgrade in Package Manager UI)**Recommend**

Find the manifest.json file in the Packages folder of your project and edit it to look like this:
``` js
{
  "scopedRegistries": [
    {
      "name": "My Registry",
      "url": "https://registry.npmjs.org",
      "scopes": [
        "com.litefeel"
      ]
    }
  ],
  "dependencies": {
    "com.litefeel.bitmapfontimporter": "3.3.0",
    ...
  }
}
```

#### Using git

Find the manifest.json file in the Packages folder of your project and edit it to look like this:
``` js
{
  "dependencies": {
    "com.litefeel.bitmapfontimporter": "https://github.com/litefeel/Unity-BitmapFontImporter.git#3.3.0",
    ...
  }
}
```

#### Using .zip file (for Unity 5.0+)

1. Download `Source code` from [Releases](https://github.com/litefeel/Unity-BitmapFontImporter/releases)
2. Extract the package into your Unity project


## How to Use?

Please see [here][howtouse]


## Support

- Create issues by [issues][issues] page
- Send email to me: <litefeel@gmail.com>

[1]: http://kvazars.com/littera/ (littera)
[2]: http://www.bmglyph.com (bmGlyph)
[3]: https://71squared.com/glyphdesigner (Glyph Designer 2)
[4]: http://renderhjs.net/shoebox/ (ShoeBox)
[5]: http://www.angelcode.com/products/bmfont/ (Bitmap Font Generator)
[bfi]: https://github.com/litefeel/Unity-BitmapFontImporter (BitmapFontImporter)
[issues]: https://github.com/litefeel/Unity-BitmapFontImporter/issues (BitmapFontImporter issues)
[howtouse]: https://github.com/litefeel/Unity-BitmapFontImporter/wiki/How-to-use (BitmapFontImporter How to use)