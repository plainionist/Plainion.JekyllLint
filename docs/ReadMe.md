
Do you use static code analysis for your code like PcLint, Roslyn or EsLint?

Looking for a similar quality assurance for your articles and posts?

Plainion.JekyllLint to the rescue ...

## Usage

- download the [latest release](https://github.com/plainionist/Plainion.JekyllLint/releases) and unpack it somewhere
- either run from command line as follows

```
d:\bin\Plainion.JekyllLint\Plainion.JekyllLint.exe <folder to markdown files>
```

- or integrate into your Visual Studio project

```xml
<Target Name="AfterBuild">  
  <Exec Command="D:\bin\Plainion.JekyllLint\Plainion.JekyllLint.exe FOLDER_TO_MARKDOWN_FILES" />
</Target>  
```

## Rules

### JL0001

Each page should have a title.

### JL0002

Titles which are too long might be shortened by search engines. 

See also: https://moz.com/learn/seo/title-tag

### JL0003

Content which is too short might be down ranked by search engines as not important enough.

### JL0004

A page should have a description. It will be included in the meta tag if you have Jekyll-SEO plug-in configured.

### JL0005

Descriptions which are too long might be shortened by search engines. 

See also: https://moz.com/learn/seo/meta-description
