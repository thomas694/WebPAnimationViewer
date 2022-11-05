# WebpAnimationViewer

A demo application written in C# with a simple WPF control for loading and showing an animated WebP file.
Also includes the original WPF control from MS for showing animated GIF images.

## Functionality / Usage

The demo application starts and displays the first WebP image. After five seconds it switches to the second WebP image.

Embed the control inside e.g. a dock panel and set its source property either in designer or code.

```C#
    <DockPanel Background="Black">
        <local:AnimatedWebpElement x:Name="anim" Source="C:\your.webp" />
    </DockPanel>
```
