# AutoMapper.Verifier

Quickly and simply make sure you have all the AutoMapper mappings you need, and none of the mappings that you do not need.

With AutoMapper, it is often difficult to tell when you are calling .Map(...) in a new context if that mapping already exists or if you need to create a new one. Or, when you remove a feature that is no longer needed, it is easy to accidentally leave unnecessary .CreateMap(...)s in, cluttering the code and making it difficult to tell if you can remove some classes because they appear to still be referenced somewhere.

AutoMapper.Verifier takes the guesswork out of figuring out which mappings you are using, which ones you are missing, and which ones you can finally get rid of.

## Getting Started

Reference the [NuGet package](https://www.nuget.org/packages/AutoMapperVerifier/) `Install-Package AutoMapperVerifier`

Then call `.Verifier.VerifyMappings(...)` in your project startup code.

```
using AutoMapper.Verifier;
...
Verifier.VerifyMappings();
```

### “Advanced” Usage

Throwing an error when there is a mapping issue.

```
try
{
    Verifier.VerifyMappings(ErrorActions.ThrowException);
}
catch(AutoMapperVerificationException ex)
{
    ...
}
```

Configuring different behaviors for different error types.

```
Verifier.VerifyMappings(x => 
    {
        x.OnUndeclaredMapping = ErrorActions.ThrowException;
        x.OnMultiplyDeclaredMapping = ErrorActions.ThrowException;
        x.OnUnusedMapping = ErrorActions.Ignore;
        x.OnIndeterminantMapping = ErrorActions.ThrowException;
    });
```

Manually handling mapping errors

```
var mappings = Verifier.VerifyMappings();

foreach(var mapping in mappings)
{
	if(mapping.HasErrors)
	{
		// do something
	}
}
```

## Known Issues

- Can only handle mappings where generic types are declared (ex: `.CreateMap<T1,T2>(...)` `.Map<T1,T2>(...)`). Mappings that pass in object or Type arguments cannot be determined via reflection.
	- CreateMap(...) could be resolved if this library were more deeply integrated with AutoMapper, but because it reflects over the code and cannot actually run it I am unable to ask for the type of the object at runtime.
	- Map(...) using object parameters would not work even if more deeply integrated because there is no way to know what the type will be for sure until it is actually passed in while the app is running, and a different type could actually be passed in each time it is called.
- Does not handle mappings to enumerables (ex: `.CreateMap<ICollection<T1>,ICollection<T2>>(...)`).
	- This should be fixable as is, I just have not had the time to fix it yet (Maybe you can? :D)

## Contributing

Open to any and all contributions.

## Authors

[Kevin Ackerman](https://www.linkedin.com/in/kevin-arthur-ackerman/)

## License

This project is licensed under the MIT License.
