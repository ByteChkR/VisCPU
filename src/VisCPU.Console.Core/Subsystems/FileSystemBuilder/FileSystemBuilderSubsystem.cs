using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml.Schema;

using VisCPU.Console.Core.Subsystems.BuildSystem;
using VisCPU.Console.Core.Subsystems.Project;
using VisCPU.Utility;
using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.IO.Settings;
using VisCPU.Utility.IO.Settings.Loader;
using VisCPU.Utility.Logging;
using VisCPU.Utility.SharedBase;

namespace VisCPU.Console.Core.Subsystems.FileSystemBuilder
{

    public class DriveImageFormatV1Settings
    {


        [Argument(Name = "dib:image.v1.path.prefix")]
        public string FileSystemPrefix = "";
        [Argument(Name = "dib:image.v1.size")]
        public uint DiskSize = 0;
        [Argument(Name = "dib:image.v1.unpack")]
        public bool UnpackData = true;
        [Argument(Name = "dib:image.v1.unpack.ignore")]
        public string[] UnpackIgnoreExtensions = new[] { "vbin" };
        static DriveImageFormatV1Settings()
        {
            SettingsCategory diCategory = SettingsCategories.Get("sdk.utils.disk.formats.v1", true);
            SettingsManager.RegisterDefaultLoader(new JsonSettingsLoader(), diCategory, "v1-format.json", new DriveImageFormatV1Settings());
        }
    }

    public class DriveImageBuilderSettings
    {

        [Argument(Name = "dib:input")]
        [Argument(Name = "dib:i")]
        public string[] InputFiles = new string[0];
        [Argument(Name = "dib:image.format")]
        public string[] ImageFormats = new[] { "FSv1" };

        static DriveImageBuilderSettings()
        {
            SettingsCategory diCategory = SettingsCategories.Get("sdk.utils.disk", true);
            SettingsManager.RegisterDefaultLoader(new JsonSettingsLoader(), diCategory, "builder.json", new DriveImageBuilderSettings());
        }
    }

    public class DriveImageSubsystem : ConsoleSubsystem
    {

        private static readonly List<DriveImageFormat> s_ImageFormats = new List<DriveImageFormat>{new DriveImageFormatV1()};
        protected override LoggerSystems SubSystem => LoggerSystems.DriveImageSystems;


        private void Unpack(string[] args)
        {
            string[] a = args.ToArray();
            DriveImageBuilderSettings settings = SettingsManager.GetSettings<DriveImageBuilderSettings>();
            ArgumentSyntaxParser.Parse(a, settings);

            for (int i = 0; i < settings.InputFiles.Length; i++)
            {
                string inputFile = settings.InputFiles[i];

                string format = GetOrLastFormat(settings.ImageFormats, i);
                DriveImageFormat f = s_ImageFormats.FirstOrDefault(x => x.FormatName == format);
                if (f == null)
                {
                    Log("Format '{0}' does not exist!", format);
                    continue;
                }
                ArgumentSyntaxParser.Parse(a, f.GetSettingsObjects());

                bool isDir = Directory.Exists(inputFile);

                if (f.SupportsDirectoryInput && isDir)
                {
                    f.Unpack(inputFile);
                }
                else
                {
                    string ext = Path.GetExtension(inputFile);

                    if (f.SupportedExtensions.Contains(ext))
                    {
                        if (!File.Exists(inputFile))
                        {
                            Log("File '{0}' does not exist!", inputFile);

                            continue;
                        }
                        f.Unpack(inputFile);

                    }
                    else
                    {
                        Log("Invalid Extension '{1}' for format '{0}'", f.FormatName, Path.GetExtension(inputFile));
                        continue;
                    }
                }
            }
        }

        public override void Help()
        {
        }

        public override void Run(IEnumerable<string> args)
        {
            string[] a = args.ToArray();

            if (a.FirstOrDefault() == "unpack")
            {
                Unpack(a.Skip(1).ToArray());
            }
            else if (a.FirstOrDefault() == "pack")
            {
                Pack(a.Skip(1).ToArray());
            }
        }
        private static string GetOrLastFormat(string[] array, int i) =>
            array.Length > i ? array[i] : array.LastOrDefault() ?? "FSv1";

        private void Pack(string[] args)
        {
            string[] a = args.ToArray();
            DriveImageBuilderSettings settings = SettingsManager.GetSettings<DriveImageBuilderSettings>();
            ArgumentSyntaxParser.Parse(a, settings);

            for (int i = 0; i < settings.InputFiles.Length; i++)
            {
                string inputFile = settings.InputFiles[i];

                string format = GetOrLastFormat(settings.ImageFormats, i);
                DriveImageFormat f = s_ImageFormats.FirstOrDefault(x => x.FormatName == format);
                if (f == null)
                {
                    Log("Format '{0}' does not exist!", format);
                    continue;
                }
                ArgumentSyntaxParser.Parse(a, f.GetSettingsObjects());

                bool isDir = Directory.Exists(inputFile);

                if (f.SupportsDirectoryInput && isDir)
                {
                    f.Pack(inputFile);
                }
                else
                {
                    string ext = Path.GetExtension(inputFile);

                    if (f.SupportedExtensions.Contains(ext))
                    {
                        if (!File.Exists(inputFile))
                        {
                            Log("File '{0}' does not exist!", inputFile);

                            continue;
                        }
                        f.Pack(inputFile);

                    }
                    else
                    {
                        Log("Invalid Extension '{1}' for format '{0}'", f.FormatName, Path.GetExtension(inputFile));
                        continue;
                    }
                }
            }
        }

    }

    public abstract class DriveImageFormat : VisBase
    {

        protected override LoggerSystems SubSystem => LoggerSystems.DriveImageBuilder;

        public abstract string FormatName { get; }
        public abstract string[] SupportedExtensions { get; }
        public abstract bool SupportsDirectoryInput { get; }

        public abstract void Pack( string input );
        public abstract void Unpack(string input);

        public virtual object[] GetSettingsObjects()
        {
            return new object[0];
        }

    }

    public class DriveImageFormatV1 : DriveImageFormat
    {

        private DriveImageFormatV1Settings m_Settings;
        
        public override void Unpack( string input )
        {
            if (!File.Exists(input))
            {
                Log("Invalid Format: {0}", Path.GetExtension(input));
                return;
            }

                
            string fullPath = Path.GetFullPath(input);

            string output = Path.Combine(
                                         Path.GetDirectoryName(fullPath),
                                         Path.GetFileNameWithoutExtension(fullPath) + ".zip"
                                        );

            if (File.Exists(output)) File.Delete(output);
            ZipArchive ret = ZipFile.Open( output, ZipArchiveMode.Create );

            FileStream fs = File.OpenRead(input);
            long startPos;
            while ( true )
            {
                if ( fs.Position == fs.Length )
                {
                    break;
                }
                startPos = fs.Position;
                byte[] numBuffer = new byte[sizeof(uint)*3];
                fs.Read(numBuffer, 0, numBuffer.Length);
                uint strLen = BitConverter.ToUInt32(numBuffer, 0) * sizeof(uint);

                if (strLen == 0)
                {
                    break;
                }

                uint dataLen = BitConverter.ToUInt32(numBuffer, sizeof(uint)) * sizeof(uint);
                uint deleted = BitConverter.ToUInt32(numBuffer, sizeof(uint)*2);

                if ( deleted != 0 )
                    continue;

                byte[] strBuffer = new byte[strLen];
                fs.Read(strBuffer, 0, strBuffer.Length);
                string name = new string( strBuffer.ToUInt().Select( x => ( char ) x ).ToArray() );

                ZipArchiveEntry e = ret.CreateEntry( name );
                Stream s = e.Open();
                byte[] buffer = new byte[1024 * 1024];
                int read=0;
                do
                {
                    int r = fs.Read( buffer, 0, Math.Min(buffer.Length, (int)dataLen-read));
                    if(r==0)break;

                    read =+r;



                    s.Write(buffer, 0, read);
                }
                while (read < dataLen);

                fs.Position = startPos + 3*sizeof(uint) + strLen + dataLen;

                s.Close();
            }

            fs.Close();
            ret.Dispose();
        }

        public override object[] GetSettingsObjects()
        {
            m_Settings = SettingsManager.GetSettings < DriveImageFormatV1Settings >();
            return new object[] {m_Settings };
        }

        public override string FormatName => "FSv1";

        public override string[] SupportedExtensions => new[] { ".zip", ".bin" };

        public override bool SupportsDirectoryInput => true;

        public override void Pack( string input )
        {
            
            if (Directory.Exists(input))
            {
                string cacheDir = Path.Combine(VisConsole.GetCacheDirectory(SubSystem), "zip-temp");
                Directory.CreateDirectory(cacheDir);
                string newFile = Path.Combine(cacheDir, Path.GetRandomFileName() + ".zip");
                ZipFile.CreateFromDirectory(input, newFile);
                input = newFile;
            }
            else
            {
                Log("Invalid Format: {0}", Path.GetExtension(input));
                return;
            }

            ZipArchive archive = ZipFile.OpenRead(input);
            string fullPath = Path.GetFullPath(input);

            string output = Path.Combine(
                                         Path.GetDirectoryName(fullPath),
                                         Path.GetFileNameWithoutExtension(fullPath) + ".bin"
                                        );

            FileStream fs = File.Create(output);

            if (m_Settings.DiskSize != 0)
            {
                fs.SetLength(m_Settings.DiskSize);
            }

            foreach (ZipArchiveEntry file in archive.Entries)
            {
                string fsPath = m_Settings.FileSystemPrefix + file.FullName;

                Log("File: '{0}' => '{1}'", file.FullName, fsPath);
                uint fileSize = m_Settings.UnpackData ? (uint)file.Length : (uint)file.Length / sizeof(uint);
                fs.Write(BitConverter.GetBytes((uint)fsPath.Length), 0, sizeof(uint));
                fs.Write(BitConverter.GetBytes(fileSize), 0, sizeof(uint));
                fs.Write(BitConverter.GetBytes(0U), 0, sizeof(uint));

                byte[] name = fsPath.Select(x => (uint)x).SelectMany(BitConverter.GetBytes).ToArray();
                fs.Write(name, 0, name.Length);

                Stream s = file.Open();
                byte[] buffer = new byte[1024 * 1024];
                int read = buffer.Length;
                do
                {
                    read = s.Read(buffer, 0, buffer.Length);

                    if (m_Settings.UnpackData && !m_Settings.UnpackIgnoreExtensions.Contains(Path.GetExtension(file.FullName)))
                    {
                        byte[] tempBuffer = buffer.Take(read).Select(x => (uint)x).SelectMany(BitConverter.GetBytes).ToArray();
                        fs.Write(tempBuffer, 0, tempBuffer.Length);
                    }
                    else
                    {
                        fs.Write(buffer, 0, read);
                    }
                }
                while (read == buffer.Length);

                s.Close();
            }

            fs.Close();
            archive.Dispose();

        }

    }

}
