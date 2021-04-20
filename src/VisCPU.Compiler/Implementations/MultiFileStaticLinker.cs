using System.Collections.Generic;
using System.IO;
using System.Linq;

using VisCPU.Compiler.Compiler;
using VisCPU.Compiler.Linking;
using VisCPU.Compiler.Linking.Events;
using VisCPU.Utility;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;
using VisCPU.Utility.IO.Settings;
using VisCPU.Utility.SharedBase;

namespace VisCPU.Compiler.Implementations
{

    public class MultiFileStaticLinker : Linker
    {

        #region Public

        public override LinkerResult Link( LinkerTarget target, Compilation compilation )
        {
            Dictionary < FileReference, LinkerTarget > tree = DiscoverCompilationTree( target );

            return ProcessOrdered( target, tree, !SettingsManager.GetSettings < LinkerSettings >().NoHiddenItems );
        }

        #endregion

        #region Private

        private Dictionary < FileReference, LinkerTarget > DiscoverCompilationTree( LinkerTarget root )
        {
            Dictionary < FileReference, LinkerTarget > allLinkerTargets =
                new Dictionary < FileReference, LinkerTarget >();

            RecursiveDiscoverCompilationTree( root, allLinkerTargets );

            return allLinkerTargets;
        }

        private void JoinDefinitions(
            IDictionary < string, AddressItem > dst,
            IDictionary < string, AddressItem > src,
            bool enableHide )
        {
            int hiddenItems = 0;
            int exportedItems = 0;
            int duplicatedItems = 0;

            foreach ( KeyValuePair < string, AddressItem > fileCompilationConstant in src )
            {
                if ( enableHide && fileCompilationConstant.Value.Hide )
                {
                    hiddenItems++;
                }
                else if ( dst.ContainsKey( fileCompilationConstant.Key ) )
                {
                    EventManager < WarningEvent >.SendEvent(
                                                            new DuplicateLinkerItemEvent( fileCompilationConstant.Key )
                                                           );

                    duplicatedItems++;
                }
                else
                {
                    dst[fileCompilationConstant.Key] = fileCompilationConstant.Value;
                    exportedItems++;
                }
            }
        }

        private void PerformLinking( LinkerResult result, List < LinkerTarget > references, bool enableHide )
        {
            //Join the Targets
            foreach ( LinkerTarget linkerTarget in references )
            {
                JoinDefinitions( result.Constants, linkerTarget.FileCompilation.Constants, enableHide );

                JoinDefinitions(
                                result.DataSectionHeader,
                                linkerTarget.FileCompilation.DataSectionHeader.ApplyOffset(
                                     ( uint ) result.DataSection.Count
                                    ),
                                enableHide
                               );

                (int, int) k = ( result.LinkedBinary.Count, linkerTarget.FileCompilation.Tokens.Count );

                result.HiddenDataSectionItems[k] =
                    SelectHidden( linkerTarget.FileCompilation.DataSectionHeader.ToArray() ).
                        ApplyOffset( ( uint ) result.DataSection.Count ).
                        ToDictionary( x => x.Key, x => x.Value );

                result.HiddenLabelItems[k] =
                    SelectHidden( linkerTarget.FileCompilation.Labels.ToArray() ).
                        ApplyOffset(
                                    ( uint ) result.LinkedBinary.Count * CpuSettings.InstructionSize
                                   ).
                        ToDictionary( x => x.Key, x => x.Value );

                result.HiddenConstantItems[k] = SelectHidden( linkerTarget.FileCompilation.Constants.ToArray() );

                result.DataSection.AddRange( linkerTarget.FileCompilation.DataSection );

                JoinDefinitions(
                                result.Labels,
                                linkerTarget.FileCompilation.Labels.ApplyOffset(
                                                                                ( uint ) result.LinkedBinary.Count *
                                                                                CpuSettings.InstructionSize
                                                                               ),
                                enableHide
                               );

                result.LinkedBinary.AddRange( linkerTarget.FileCompilation.Tokens );
            }
        }

        private LinkerResult ProcessOrdered(
            LinkerTarget root,
            Dictionary < FileReference, LinkerTarget > mapping,
            bool enableHide )
        {
            Stack < FileReference > todo =
                new Stack < FileReference >( mapping.Keys.Where( x => x.File != root.FileCompilation.Reference.File ) );

            LinkerResult result = new LinkerResult(
                                                   mapping.Values.ToArray(),
                                                   new Dictionary < string, AddressItem >(),
                                                   new Dictionary < string, AddressItem >(),
                                                   new Dictionary < string, AddressItem >(),
                                                   new List < AToken[] >(),
                                                   new uint[0]
                                                  );

            PerformLinking( result, new List < LinkerTarget > { root }, enableHide );

            while ( todo.Count != 0 )
            {
                FileReference nextFile = todo.Pop();
                LinkerTarget fileTarget = mapping[nextFile];
                PerformLinking( result, new List < LinkerTarget > { fileTarget }, enableHide );
            }

            return result;
        }

        private void RecursiveDiscoverCompilationTree(
            LinkerTarget root,
            Dictionary < FileReference, LinkerTarget > targets )
        {
            if ( root.FileCompilation.FileReferences.Count != 0 )
            {
                foreach ( FileReference reference in root.FileCompilation.FileReferences )
                {
                    if ( targets.ContainsKey( reference ) )
                    {
                        continue;
                    }

                    if ( File.Exists( reference.File ) )
                    {
                        FileCompilation fc = new FileCompilation( reference );

                        LinkerTarget t = new LinkerTarget( fc, fc.Reference.LinkerArguments );
                        targets[reference] = t;
                        RecursiveDiscoverCompilationTree( t, targets );
                    }
                    else
                    {
                        LinkerInfo info = LinkerInfo.Load( reference.File );
                        FileCompilation fc = new FileCompilation( info );
                        LinkerTarget t = new LinkerTarget( fc, new object[0] );
                        targets[reference] = t;
                    }
                }
            }
        }

        private Dictionary < string, AddressItem > SelectHidden( KeyValuePair < string, AddressItem >[] items )
        {
            return items.Where( x => x.Value.Hide ).ToDictionary( x => x.Key, x => x.Value );
        }

        #endregion

    }

}
