class NUnit
	def self.run(attributes)
		tool = attributes.fetch(:tool, "lib/nunit/nunit-console.exe").to_absolute()
		assemblies = attributes.fetch(:assemblies)
		include = attributes.fetch(:include, nil)
		include = "/include=#{include}" if include
		
		for assembly in assemblies
			sh "#{tool.escape} #{assembly} #{include if include}" 
		end
	end
end