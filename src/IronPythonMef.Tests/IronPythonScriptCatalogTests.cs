﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using NUnit.Framework;

namespace IronPythonMef.Tests
{
    [TestFixture]
    public class IronPythonScriptCatalogTests
    {
        private readonly IEnumerable<Type> _injectTypes = new List<Type>
        {
            typeof (IActOnItem),
            typeof (IItemSource),
            typeof (BasePythonItemSource)
        };

        private ScriptEngine _engine;

        [SetUp]
        public void Setup()
        {
             _engine = Python.CreateEngine();
            
        }
        [Test]
        public void CatalogCanExportValueBasedOnDecorator()
        {
            const string pythonCode =
@"
@export(IItemSource)
class StringItemSource(BasePythonItemSource):
    def GetAllItems(self):
        return [""Item 1"", ""Item 2"", ""Item 3""]
";

            var catalog = new IronPythonScriptCatalog(_engine, new StringReader(pythonCode), _injectTypes);
            var container = new CompositionContainer(catalog);
            var itemSources = container.GetExportedValues<IItemSource>().ToList();
            Assert.AreEqual(1, itemSources.Count);
            var strings = itemSources[0].GetItems().Result.Cast<string>().ToList();
            Assert.AreEqual(3, strings.Count);
            Assert.AreEqual("Item 1", strings[0]);
            Assert.AreEqual("Item 2", strings[1]);
            Assert.AreEqual("Item 3", strings[2]);
        }
    }
}