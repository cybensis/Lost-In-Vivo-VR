# LIVVR - Lost In Vivo VR
> Full 6DOF VR mod for Lost In Vivo

<!-- [![Build Status][travis-image]][travis-url]
[![Downloads Stats][npm-downloads]][npm-url] -->

At its current stage this mod enables full 6DOF VR in Lost In Vivo with motion controls planned for the future. It uses Raicuparta's Unity VR patcher to enable VR and is patched by myself to enable VR controller support and fix various other issues.



## Installation

1. Download the most recent release.
2. Extract the contents to the root directory of Lost In Vivo (the directory containing LIV.exe) and overwrite anything that needs to be overwritten.
3. The first time launching the game after installing the mod may or may not work, if it doesn't just launch it again.


## Development setup

If you want to help with developing this mod or build it yourself, start by downloading the source code, then in a terminal navigate to the source code folder and enter

```sh
dotnet restore ./
```

Then opening with your favourite IDE should be enough to begin developing or building this mod.


## Release History

* 1.0.0
    * Initial release of a working version of Lost In Vivo VR

## Support

* So far this mod has only been tested on a Quest 2 and with the Oculus Touch controllers. It should work for other VR headsets but it will not work for other VR controllers until you add your own controller mapping scheme.


## Credits

* Big thanks to [PinkMilkProductions](https://www.youtube.com/channel/UCvZLpwlyxn6lFYXKsBl6qHg) for his Unity VR modding series 
* Thanks to [Raicuparta](https://github.com/Raicuparta) for his Unity VR Patcher

<!-- Markdown link & img dfn's -->
<!-- [npm-image]: https://img.shields.io/npm/v/datadog-metrics.svg?style=flat-square
[npm-url]: https://npmjs.org/package/datadog-metrics
[npm-downloads]: https://img.shields.io/npm/dm/datadog-metrics.svg?style=flat-square
[travis-image]: https://img.shields.io/travis/dbader/node-datadog-metrics/master.svg?style=flat-square
[travis-url]: https://travis-ci.org/dbader/node-datadog-metrics
[wiki]: https://github.com/yourname/yourproject/wiki -->
