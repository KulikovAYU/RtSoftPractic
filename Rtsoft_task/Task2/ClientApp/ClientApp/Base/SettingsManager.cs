using System;
using System.IO;
using System.Threading.Tasks;
using ClientApp.Utils;

namespace ClientApp.Base
{
    public class SettingsManager
    {

        private static readonly Lazy<SettingsManager> Lazy = new(() => new SettingsManager());

        public static SettingsManager GetInstance() => Lazy.Value;
        
        public async Task<bool> SavePrefAsync<T>(T srcObj, string path)
        {
            ISerrializer serrializer = null;
            string ext = Path.GetExtension(path);
            
            switch (ext)
            {
                case ".json":
                {
                    serrializer = new JsonSerrializer();
                    break;
                }
            }
            
            if (serrializer == null)
                return false;

            try
            {
                string sData = serrializer.Serrialize(srcObj);
                return await FileUtils.WriteAllTextAsync(sData, path);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        
        
        public async Task<T?> LoadPrefAsync<T>(string path)
        {
            string ext = Path.GetExtension(path);

            ISerrializer serrializer = null;   
          
            switch (ext)
            {
                case ".json":
                {
                    serrializer = new JsonSerrializer();
                    break;
                }
            }

            if (serrializer == null) return default;
            
            string strContent = await FileUtils.ReadAllTextAsync(path);
            return  serrializer.DeSerrialize<T>(strContent) ?? throw new InvalidOperationException("usuported format");
        }
        
        private SettingsManager()
        {
           
        }

    }
}