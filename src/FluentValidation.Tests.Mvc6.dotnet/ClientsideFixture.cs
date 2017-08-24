namespace FluentValidation.Tests.AspNetCore {
	using System;
	using System.Linq;
	using System.Threading.Tasks;
	using System.Xml.Linq;

	public class ClientsideFixture<TStartup> : WebAppFixture<TStartup> where TStartup : class {

		public async Task<XDocument> GetClientsideMessages(string action = "/Clientside/Inputs") {
			var output = await GetResponse(action);
			return XDocument.Parse(output);
		}

		public async Task<string> GetClientsideMessage(string name, string attribute) {
			var doc = await GetClientsideMessages();
			var elem = doc.Root.Elements("input")
				.Where(x => x.Attribute("name").Value == name).SingleOrDefault();

			if (elem == null) {
				throw new Exception("Could not find element with name " + name);
			}

			var attr = elem.Attribute(attribute);

			if (attr == null || string.IsNullOrEmpty(attr.Value)) {
				throw new Exception("Could not find attr " + attribute);
			}

			return attr.Value;
		}

		public async Task<string[]> RunRulesetAction(string action) {

			var doc = await GetClientsideMessages(action);

			var elems = doc.Root.Elements("input")
				.Where(x => x.Attribute("name").Value.StartsWith("CustomName"));

			var results = elems.Select(x => x.Attribute("data-val-required"))
				.Where(x => x != null)
				.Select(x => x.Value)
				.ToArray();

			return results;
		}


	}
}