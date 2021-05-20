using System.Collections.Generic;

namespace VPP.Importer
{

    public abstract class VPPMakro
    {

        public readonly string Name;
        public readonly List<VPPMakroParameter> Parameters;

        protected VPPMakro(string name, List<VPPMakroParameter> parameters)
        {
            Name = name;
            Parameters = parameters;
        }

        #region Public

        public abstract string GenerateValue(string[] args);
        #endregion

        #region Private


        #endregion

    }

}
