class SevenZip
	attr_accessor :tool, :args, :zipName
	
	def initialize(params = {})
		@tool = params.fetch(:tool).to_absolute
		@args = params.fetch(:args, 'a')
		@zipName = params.fetch(:zip_name).to_absolute
	end
	
	def zip(params = {})
		files = params.fetch(:files)
		
		SevenZip.zip_files @tool, @args, @zipName, files
	end
	
	def self.zip(params = {})
		tool = params.fetch(:tool)
		args = params.fetch(:args, 'a')
		zipName = params.fetch(:zip_name).to_absolute
		files = params.fetch(:files)
		
		zip_files tool, args, zipName, files
	end
	
	def self.unzip(params= {})
		tool = params.fetch(:tool)
		args = params.fetch(:args, 'x -y')
		zipName = params.fetch(:zip_name).to_absolute
		destination = params.fetch(:destination, '.')
		
		sevenZip = tool.to_absolute
		
		sh "#{sevenZip.escape} #{args} #{zipName.escape} -o#{destination.escape}"
	end
	
	def self.zip_files(tool, args, zipName, files)
		return if files.empty?
		
		FileUtils.mkdir_p zipName.dirname
		files.map! do |f|
			f.escape
		end
		
		sevenZip = tool.to_absolute
				
		sh "#{sevenZip.escape} #{args} #{zipName.escape} #{files}"
	end
end