using Checkers.Properties;
using Checkers.ViewModels;
using CheckersBase.BrainBase;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace Checkers.Models
{
    internal class BrainLoader
    {
        public static List<BrainBase> LoadBrainsFromDll(string fileName)
        {
	        try
	        {
				return Assembly.LoadFrom(fileName)
				.GetTypes()
				.Where(t => t.BaseType == typeof(BrainBase))
				.Select(t => (BrainBase)Activator.CreateInstance(t))
				.ToList();
	        }
	        catch (Exception ex)
	        {
		        MessageBox.Show("Ошибка загрузки ботов из файла: " + fileName + Environment.NewLine + "Ошибка: " +
		                        ex.ToString());
		        return null;
	        }
            
        }

        public static ObservableCollection<BrainViewModel> LoadBrains() 
        {
            var ret = new ObservableCollection<BrainViewModel>();

            var folder = Settings.Default.BotFolder;
            if (string.IsNullOrEmpty(folder))
                folder = Directory.GetCurrentDirectory();
            
            if (!Directory.Exists(folder))
            {
                MessageBox.Show("Не выбрана директория ботов!");
                return ret;
            }

            var dllFiles = Directory.GetFiles(folder, "*.dll", SearchOption.AllDirectories);
            foreach (var dllFile in dllFiles)
            {
                var brains = LoadBrainsFromDll(dllFile);
                foreach(var brain in brains)
                    ret.Add(new BrainViewModel(brain));
            }

            return ret;
        }

    }
}
