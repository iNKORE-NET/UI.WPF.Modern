# Banner

Ho do you like this project so far? If this project is useful to you, please consider giving it a star on GitHub, sharing it with others and add a banner with a link to this project in your README.

You can use the banner in two sources:

- Github URL (1280w): `https://github.com/iNKORE-NET/UI.WPF.Modern/blob/main/assets/images/banners/UI.WPF.Modern_Main_1280w.png?raw=true`

- Github URL (2560w): `https://github.com/iNKORE-NET/UI.WPF.Modern/blob/main/assets/images/banners/UI.WPF.Modern_Main_2560w.png?raw=true`

- iNKORE.UI.WPF.Modern Built-in: `ThemeManager.BannerUri_1280w`

## Markdown Code

For example, in the README of your project, you can add the following markdown code:

```markdown
<a href="https://docs.inkore.net/ui-wpf-modern/introduction">
  <img src="https://github.com/iNKORE-NET/UI.WPF.Modern/blob/main/assets/images/banners/UI.WPF.Modern_Main_1280w.png?raw=true">
</a>
```

This will display the following banner:

<a href="https://docs.inkore.net/ui-wpf-modern/introduction">
  <img src="https://github.com/iNKORE-NET/UI.WPF.Modern/blob/main/assets/images/banners/UI.WPF.Modern_Main_1280w.png?raw=true">
</a>

You can also use a higher resolution (2560w) banner by replacing `1280w` with `2560w` in the URL.

## WPF Code

In your WPF application, you can use the `ui:ThemeManager.BannerUri_1280w` attached property to display the banner:

```xml
<Image x:Name="headerImage" Stretch="Uniform">
    <Image.Source>
        <BitmapImage UriSource="{x:Static ui:ThemeManager.BannerUri_1280w}"/>
    </Image.Source>
</Image>
```

## Remarks

We strongly recommend you to add the banner image as a LINK in your project, instead of downloading the image and adding it to your repository. This way, you will always have the latest version of the banner.