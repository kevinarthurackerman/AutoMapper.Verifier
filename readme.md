# AutoMapper.Verifier

Quickly and simply make sure you have all the AutoMapper mappings you need, and none of the mappings that you do not need.

## Getting Started

Reference the NuGet package, call `.Verifier.VerifyMappings(...)` in your project startup code.

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

## Known Issues

- Can only handle mappings where generic types are declared (ex: `.CreateMap<T1,T2>(...)` `.Map<T1,T2>(...)`).
	- CreateMap could be resolved if this library were more deeply integrated with AutoMapper, but because it reflects over the code and cannot actually run it I am unable to ask for the type of the object at runtime.
- Does not handle mappings to enumerables (ex: .CreateMap<ICollection<T1>,ICollection<T2>>(...)).
	- This should be fixable as is, I just have not had the time to fix it yet (Maybe you can? :D)

## Contributing

Open to any and all contributions.

## Authors

Kevin Ackerman

## License

This project is licensed under the MIT License.
