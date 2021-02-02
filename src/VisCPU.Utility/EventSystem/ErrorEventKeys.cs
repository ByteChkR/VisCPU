namespace VisCPU.Utility.EventSystem
{

    public static class ErrorEventKeys
    {

        public static readonly string s_HlConstVarDuplicateDef = "hl-const-var-duplicate-definition";

        public static readonly string s_HlCompilerDynamicVariablesNotSupported =
            "hl-compiler-dynamic-variables-not-supported";

        public static readonly string s_HlVarDuplicateDef = "hl-var-duplicate-definition";
        public static readonly string s_HlMemberDuplicateDef = "hl-member-duplicate-definition";
        public static readonly string s_HlInvalidMemberModifiers = "hl-invalid-member-modifiers";
        public static readonly string s_HlMemberNotFound = "hl-member-not-found";
        public static readonly string s_HlTypeDuplicateDef = "hl-type-duplicate-definition";
        public static readonly string s_HlFunctionArgumentMismatch = "hl-function-argument-mismatch";
        public static readonly string s_HlFunctionNotFound = "hl-function-not-found";
        public static readonly string s_HlTypeNotFound = "hl-type-not-found";
        public static readonly string s_HlStaticParseFailed = "hl-static-parse-failed";
        public static readonly string s_HlVariableNotFound = "hl-variable-not-found";
        public static readonly string s_HlCompilerNotFound = "hl-compiler-not-found";
        public static readonly string s_HlInvalidToken = "hl-invalid-token";

        public static readonly string s_InstrDuplivateOpCode = "instr-duplicate-op-code";
        public static readonly string s_InstrOpNotFound = "instr-op-not-found";

        public static readonly string s_AsmGenTooManyArgs = "asm-gen-too-many-arguments";
        public static readonly string s_AsmGenTokenRecognitionFailure = "asm-gen-token-recognition-failure";

        public static readonly string s_VasmInvalidConstantDefinition = "vasm-invalid-const-definition";
        public static readonly string s_VasmInvalidDataDefinition = "vasm-invalid-data-definition";
        public static readonly string s_VasmParserInvalidCharValue = "vasm-parser-invalid-char-value";
        public static readonly string s_VasmParserInvalidNumberValue = "vasm-parser-invalid-number-value";

        public static readonly string s_LinkerFileReferencesUnsupported = "linker-file-references-unsupported";

        public static readonly string s_GenericFileInvalid = "generic-file-invalid";
        public static readonly string s_GenericFileNotFound = "generic-file-not-found";

        public static readonly string s_SettingsDuplicateCategory = "settings-duplicate-category";
        public static readonly string s_SettingsLoaderNotFound = "settings-loader-not-found";
        public static readonly string s_SettingsIoDisabled = "settings-io-disabled";

        public static readonly string s_UriResolverFailure = "uri-resolver-failure";

        public static readonly string s_HfsInvalidCommand = "hfs-invalid-command";
        public static readonly string s_BenchmarkDeviceInvalidUsage = "benchmark-device-invalid-usage";

        public static readonly string s_VasmBridgeInvalidArguments = "vasm-bridge-invalid-arguments";
        public static readonly string s_LinkerImporterInvalidArguments = "linker-importer-invalid-arguments";
        public static readonly string s_LinkerImporterNoOffset = "linker-importer-no-offset";
        public static readonly string s_ImporterNotFound = "importer-not-found";
        public static readonly string s_ImporterInvalidType = "importer-type-invalid";

        public static readonly string s_ModuleManagerUnsupportedFeature = "module-manager-feature-unsupported";
        public static readonly string s_ModuleFileNotFound = "module-file-not-found";

        public static readonly string s_HfsReadFailure = "hfs-read-failure";
        public static readonly string s_OriginUrlSchemeUnsupported = "origin-url-scheme-unsupported";

    }

}
