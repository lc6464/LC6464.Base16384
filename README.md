# LC6464.Base16384

[NuGet 包](https://www.nuget.org/packages/LC6464.Base16384 "NuGet.Org")
[GitHub 项目](https://github.com/lc6464/LC6464.Base16384 "GitHub.Com")

Base16384 编解码器的 .NET 实现。
参考：[fumiama/base16384](https://github.com/fumiama/base16384 "GitHub: fumiama/base16384")
使用：[lc6464/Base16384.Net](https://github.com/lc6464/Base16384.Net "GitHub: lc6464/Base16384.Net")

## 使用方法
`Example.csproj`:
``` xml
﻿<Project Sdk="Microsoft.NET.Sdk">
	<PropertyGroup>
		<OutputType>Exe</OutputType>
		<TargetFramework>net7.0</TargetFramework>
		<Nullable>enable</Nullable>
		<ImplicitUsings>enable</ImplicitUsings>
		<!-- 一些东西 -->
	</PropertyGroup>
	<ItemGroup>
		<PackageReference Include="LC6464.Base16384" Version="2.1.0" />
		<!-- PackageReference，建议使用 Visual Studio 或 dotnet cli 等工具添加 -->
	</ItemGroup>
	<ItemGroup>
		<Using Include="LC6464.Base16384" />
		<!-- 一些东西 -->
	</ItemGroup>
</Project>
```

`Program.cs`:
``` csharp
Console.Write("请输入要编码的字符串：");
var input = Console.ReadLine() ?? "";
var data = Encoding.UTF8.GetBytes(input);

var result = Base16384.EncodeToNewFile(data, new("output.txt"));
```