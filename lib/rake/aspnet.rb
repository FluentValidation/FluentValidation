class AspNetCompiler
	def self.compile(attributes)
		physicalDir = attributes.fetch(:physical_dir)
		virtualDir = attributes.fetch(:virtual_dir, 'This_Value_Is_Not_Used')
		
		frameworkDir = File.join(ENV['windir'].dup, 'Microsoft.NET', 'Framework', 'v2.0.50727')
		aspNetCompiler = File.join(frameworkDir, 'aspnet_compiler.exe')

		sh "#{aspNetCompiler.escape} -nologo -errorstack -c -p #{physicalDir.escape} -v #{virtualDir}"
	end
end