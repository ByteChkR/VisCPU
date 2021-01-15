namespace VisCPU.Utility.EventSystem
{

    public static class ErrorEventKeys
    {

        public const string HL_CONST_VAR_DUPLICATE_DEF = "hl-const-var-duplicate-definition";
        public const string HL_COMPILER_DYNAMIC_VARIABLES_NOT_SUPPORTED = "hl-compiler-dynamic-variables-not-supported";
        public const string HL_VAR_DUPLICATE_DEF = "hl-var-duplicate-definition";
        public const string HL_MEMBER_DUPLICATE_DEF = "hl-member-duplicate-definition";
        public const string HL_INVALID_MEMBER_MODIFIERS = "hl-invalid-member-modifiers";
        public const string HL_MEMBER_NOT_FOUND = "hl-member-not-found";
        public const string HL_TYPE_DUPLICATE_DEF = "hl-type-duplicate-definition";
        public const string HL_FUNCTION_ARGUMENT_MISMATCH = "hl-function-argument-mismatch";
        public const string HL_FUNCTION_NOT_FOUND = "hl-function-not-found";
        public const string HL_TYPE_NOT_FOUND = "hl-type-not-found";
        public const string HL_VARIABLE_NOT_FOUND = "hl-variable-not-found";
        public const string HL_COMPILER_NOT_FOUND = "hl-compiler-not-found";
        public const string HL_INVALID_TOKEN = "hl-invalid-token";

        public const string INSTR_DUPLIVATE_OP_CODE = "instr-duplicate-op-code";
        public const string INSTR_OP_NOT_FOUND = "instr-op-not-found";

        public const string ASM_GEN_TOO_MANY_ARGS = "asm-gen-too-many-arguments";
        public const string ASM_GEN_TOKEN_RECOGNITION_FAILURE = "asm-gen-token-recognition-failure";

        public const string VASM_INVALID_CONSTANT_DEFINITION = "vasm-invalid-const-definition";
        public const string VASM_INVALID_DATA_DEFINITION = "vasm-invalid-data-definition";
        public const string VASM_PARSER_INVALID_CHAR_VALUE = "vasm-parser-invalid-char-value";
        public const string VASM_PARSER_INVALID_NUMBER_VALUE = "vasm-parser-invalid-number-value";

        public const string LINKER_FILE_REFERENCES_UNSUPPORTED = "linker-file-references-unsupported";

        public const string GENERIC_FILE_INVALID = "generic-file-invalid";
        public const string GENERIC_FILE_NOT_FOUND = "generic-file-not-found";

        public const string SETTINGS_LOADER_NOT_FOUND = "settings-loader-not-found";
        public const string SETTINGS_IO_DISABLED = "settings-io-disabled";

        public const string URI_RESOLVER_FAILURE = "uri-resolver-failure";

        public const string HFS_INVALID_COMMAND = "hfs-invalid-command";

        public const string VASM_BRIDGE_INVALID_ARGUMENTS = "vasm-bridge-invalid-arguments";
        public const string LINKER_IMPORTER_INVALID_ARGUMENTS = "linker-importer-invalid-arguments";
        public const string LINKER_IMPORTER_NO_OFFSET = "linker-importer-no-offset";
        public const string IMPORTER_NOT_FOUND = "importer-not-found";
        public const string IMPORTER_INVALID_TYPE = "importer-type-invalid";

        public const string MODULE_MANAGER_UNSUPPORTED_FEATURE = "module-manager-feature-unsupported";

        public const string ORIGIN_URL_SCHEME_UNSUPPORTED = "origin-url-scheme-unsupported";

    }

}
