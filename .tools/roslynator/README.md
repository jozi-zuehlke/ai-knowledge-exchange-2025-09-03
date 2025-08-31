# Roslynator CLI

We require this dedicated directory to run the roslynator CLI from since when calling it with a newer version of dotnet than 8 it will fail with an error like this:

```log
System.AggregateException: One or more errors occurred. (Could not load file or     │
 │    assembly 'System.Composition.AttributedModel, Version=9.0.0.6, Culture=neutral,     │
 │    PublicKeyToken=b03f5f7f11d50a3a'. The system cannot find the file specified.        │
 │    )                                                                                   │
 │     ---> System.IO.FileNotFoundException: Could not load file or assembly              │
 │    'System.Composition.AttributedModel, Version=9.0.0.6, Culture=neutral,              │
 │    PublicKeyToken=b03f5f7f11d50a3a'. The system cannot find the file specified.        │
 │                                                                                        │
 │    File name: 'System.Composition.AttributedModel, Version=9.0.0.6, Culture=neutral,   │
 │    PublicKeyToken=b03f5f7f11d50a3a' 
```
