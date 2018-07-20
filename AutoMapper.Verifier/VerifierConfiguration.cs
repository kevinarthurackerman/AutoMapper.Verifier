namespace AutoMapper.Verifier
{
    public class VerifierConfiguration
    {
        internal VerifierConfiguration() { }

        public ErrorActions OnUndeclaredMapping { get; set; } = ErrorActions.LogError;
        public ErrorActions OnMultiplyDeclaredMapping { get; set; } = ErrorActions.LogError;
        public ErrorActions OnUnusedMapping { get; set; } = ErrorActions.LogError;
        public ErrorActions OnIndeterminantMappingCreation { get; set; } = ErrorActions.LogError;
        public ErrorActions OnIndeterminantMappingUsage { get; set; } = ErrorActions.LogError;

        internal void SetAllErrorActions(ErrorActions onError)
        {
            OnUndeclaredMapping = onError;
            OnMultiplyDeclaredMapping = onError;
            OnUnusedMapping = onError;
            OnIndeterminantMappingCreation = onError;
            OnIndeterminantMappingUsage = onError;
        }
    }
}
