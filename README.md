Coverity Publisher
==================

<img align="right" width="256px" height="256px" src="http://img.csmac.nz/PublishCoverity-256.svg">

[![License](http://img.shields.io/:license-mit-blue.svg)](http://csmacnz.mit-license.org)
[![NuGet](https://img.shields.io/nuget/v/PublishCoverity.svg)](https://www.nuget.org/packages/PublishCoverity)
[![NuGet](https://img.shields.io/nuget/dt/PublishCoverity.svg)](https://www.nuget.org/packages/PublishCoverity)
[![Badges](http://img.shields.io/:badges-12/12-ff6799.svg)](https://github.com/badges/badgerbadgerbadger)

[![AppVeyor Build status](https://img.shields.io/appveyor/ci/MarkClearwater/coveritypublisher.svg)](https://ci.appveyor.com/project/MarkClearwater/coveritypublisher)
[![Travis Build Status](https://img.shields.io/travis/csMACnz/coveritypublisher.svg)](https://travis-ci.org/csMACnz/coveritypublisher)

[![Coverage Status](https://img.shields.io/coveralls/csMACnz/coveritypublisher.svg)](https://coveralls.io/r/csMACnz/coveritypublisher?branch=master)
[![Coverity Scan Build Status](https://scan.coverity.com/projects/4354/badge.svg)](https://scan.coverity.com/projects/4354)

[![Stories in Ready](https://badge.waffle.io/csmacnz/coveritypublisher.png?label=ready&title=Ready)](https://waffle.io/csmacnz/coveritypublisher)
[![Stories in progress](https://badge.waffle.io/csmacnz/coveritypublisher.png?label=in%20progress&title=In%20Progress)](https://waffle.io/csmacnz/coveritypublisher)
[![Issue Stats](http://www.issuestats.com/github/csMACnz/coveritypublisher/badge/pr)](http://www.issuestats.com/github/csMACnz/coveritypublisher)
[![Issue Stats](http://www.issuestats.com/github/csMACnz/coveritypublisher/badge/issue)](http://www.issuestats.com/github/csMACnz/coveritypublisher)


A small utility app to publish your coverity results.

Usage
-----

After you have created your zip file, simply execute the following:

    PublishCoverity compress -o coverity.zip -i cov-int

    PublishCoverity publish -z coverity.zip -r USER/REPO -t TOKEN -e test@example.com -d "My cool app for X" --codeVersion "1.2.3-alpha"

Note you don't want to embed your token inside your build files, use your build servers secure variable mechanism instead.
