
Do you use static code analysis for your code like PcLint, Roslyn or EsLint?

Looking for a similar quality assurance for your articles and posts?

Plainion.JekyllLint to the rescue ...

# Installation & Usage

- you need to have .Net Core 2.1 or newer installed
- download the [latest release](https://github.com/plainionist/Plainion.JekyllLint/releases) and unpack it somewhere
- either run from command line as follows

```
dotnet \bin\Plainion.JekyllLint\Plainion.JekyllLint.dll <folder to markdown files>
```

- or integrate into your Visual Studio project

```xml
<Target Name="AfterBuild">  
  <Exec Command="dotnet \bin\Plainion.JekyllLint\Plainion.JekyllLint.dll FOLDER_TO_MARKDOWN_FILES" />
</Target>  
```

# Rules

## JL0001

Each page should have a title.

## JL0002

Titles which are too long might be shortened by search engines. 
Titles should not be longer than 60 characters.

See also: <https://moz.com/learn/seo/title-tag>

## JL0003

Content which is too short might be down ranked by search engines as not important enough.

Ideally the content of a blog post is between 2000 and 2400 words long.


## JL0004

A page should have a description. It will be included in the meta tag if you have Jekyll-SEO plug-in configured.

## JL0005

Descriptions which are too long might be shortened by search engines. 

A descriptions should have between 50 and 300 characters.

See also: <https://moz.com/learn/seo/meta-description>

## JL0006

Images should have "alt" text. This is not only important in case the image cannot be
displayed, it also impacts SEO.

To add an alt text with markdown use the following syntax:

```MarkDown
![alt text](http://some-domain.net/myimage.png)
```

With HTML you can add an alt text as follows

```HTML
<img src="http://some-domain.net/myimage.png" alt="alt text"/>
```

See also: <https://moz.com/learn/seo/alt-text>

## JL0007

Images should have "title" text. This is not only serving as tool-tip, it also impacts SEO.

To add a title text with markdown use the following syntax:

```MarkDown
![](http://some-domain.net/myimage.png "title text")
```

With HTML you can add a title as follows

```HTML
<img src="http://some-domain.net/myimage.png" title="title text"/>
```

See also: <https://yoast.com/image-seo-alt-tag-and-title-tag-optimization/>

# Configuration

You can disable a rule for a specific article or post by adding it to the front matter variable ```lint-nowarn``` like this:

```yaml
---
lint-nowarn: JL0003, JL0002
---
```

You can instruct Plainion.JekyllLint to treat all warnings as errors by passing ```-warning-to-error``` command line switch.

You can instruct Plainion.JekyllLint to treat all errors as warnings by passing ```-error-to-warning``` command line switch.

# Contribution

You have ideas to improve Plainion.JekyllLint? You are welcome to contribute!

Plainion.JekyllLint is hosted on [GitHub](https://github.com/plainionist/Plainion.JekyllLint). 
Just clone the repository, [setup your environment](Contribution) and get your fingers dirty ;-) 
Send me a pull request when you are done!

