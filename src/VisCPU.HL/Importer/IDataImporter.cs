﻿using VisCPU.HL.DataTypes;

namespace VisCPU.HL.Importer
{

    public interface IDataImporter : IImporter
    {
        IExternalData[] ProcessImport( HlCompilation compilation, string input );
    }

}
