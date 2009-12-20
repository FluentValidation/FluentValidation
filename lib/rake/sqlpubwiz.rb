class SqlPubWiz
	def self.run(attributes)
		tool = attributes.fetch(:tool)
		connectionString = attributes.fetch(:connection_string)
		outputFile = attributes.fetch(:output_file).to_absolute
		
		FileUtils.mkdir_p outputFile.dirname
		
		sqlpubwiz = tool.to_absolute
		
		sh "#{sqlpubwiz.escape} script -f -dataonly -C #{connectionString.escape} #{outputFile.escape}"
	end
end