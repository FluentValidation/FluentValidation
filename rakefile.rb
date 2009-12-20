Dir.glob(File.join(File.dirname(__FILE__), 'lib/Rake/*.rb')).each do |f|
	require f
end

project_name = "FluentValidation"
solution_file = "FluentValidation.sln"
solution_file_silverlight = "FluentValidation.Silverlight.sln"
project_configuration = "release"
build_dir = "build"
test_assemblies = ["src/FluentValidation.Tests/bin/#{project_configuration}/FluentValidation.Tests.dll"]
build_silverlight = false
ncover_path = "C:/Program Files (x86)/ncover"

task :default => [ :init, :compile, :test, :deploy ]
task :ci => [ :init, :silverlight, :compile, :coverage, :package ]

task :init do
	FileUtils.rm_rf build_dir if File.exists? build_dir
	FileUtils.rm "teamcity-info.xml" if File.exists? "teamcity-info.xml"
end

task :silverlight do 
	puts "Silverlight build enabled"
	build_silverlight = true 
end

desc "Compiles solution"
task :compile do
	MSBuild.compile :project => solution_file, :properties => {
		:Configuration => project_configuration
	}
		
	if build_silverlight
		MSBuild.compile :project => solution_file_silverlight, :properties => {
			:Configuration => project_configuration
		}
	end
end

desc "Runs tests"
task :test do
	NUnit.run :assemblies => test_assemblies
end

desc "Creates release package"
task :deploy do
	
	FileUtils.mkdir_p "#{build_dir}/#{project_configuration}/FluentValidation"
	FileUtils.mkdir "#{build_dir}/#{project_configuration}/MVC"
	FileUtils.mkdir "#{build_dir}/#{project_configuration}/CommonServiceLocator"
	FileUtils.mkdir "#{build_dir}/#{project_configuration}/xVal"
	FileUtils.mkdir "#{build_dir}/#{project_configuration}/Silverlight"
	
	#main binaries
	FileUtils.cp_r "src/FluentValidation/bin/#{project_configuration}/.", "#{build_dir}/#{project_configuration}/FluentValidation"
	
	#Mvc
	FileList.new("src/FluentValidation.Mvc/bin/#{project_configuration}/FluentValidation.Mvc.*").each do |f|
		copy f, "#{build_dir}/#{project_configuration}/MVC"
	end
		
	#csl
	FileList.new \
		.include("src/FluentValidation.CommonServiceLocator/bin/#{project_configuration}/FluentValidation.CommonServiceLocator.*") \
		.include("src/FluentValidation.CommonServiceLocator/bin/#{project_configuration}/Microsoft.Practices.ServiceLocation.dll") \
		.each do |f|
			copy f, "#{build_dir}/#{project_configuration}/CommonServiceLocator"
		end

	#xval
	FileList.new(
		"src/FluentValidation.xValIntegration/bin/#{project_configuration}/FluentValidation.xValIntegration.*", 
		"src/FluentValidation.xValIntegration/bin/#{project_configuration}/xVal.dll"
	) do |f|
		copy f, "#{build_dir}/#{project_configuration}/xVal"
	end
		
	#silverlight
	if build_silverlight
		FileList.new("src/FluentValidation.Silverlight/bin/#{project_configuration}/*.{dll,pdb,xml}").each do |f|
			copy f, "#{build_dir}/#{project_configuration}/Silverlight"
		end
	end
	
	#license/changelog
	FileList.new("License.txt", "Changelog.txt").each do |f|
		copy f, "#{build_dir}/#{project_configuration}"
	end
end

desc "Creates zip package"
task :package => :deploy do
	sz = SevenZip.new :tool => "lib/7zip/7za.exe", :zip_name => "#{build_dir}/FluentValidation.zip"
	
	Dir.chdir("#{build_dir}/#{project_configuration}") do
		sz.zip :files => FileList.new("**/*.{dll,pdb,xml,txt}")
	end
end

task :coverage do

	test_assemblies.each do |assembly|
			app_assemblies = [
				"FluentValidation.Mvc", 
				"FluentValidation.xValIntegration", 
				"FluentValidation.CommonServiceLocator", 
				"FluentValidation"].join(";")
				
			NCover.run_coverage \
				:tool => "#{ncover_path}/ncover.console.exe",
				:report_dir => "#{build_dir}/Coverage",
				:working_dir => assembly.dirname,
				:application_assemblies => app_assemblies,
				:program => ENV['teamcity.dotnet.nunitlauncher'],
				:assembly => "v2.0 x86 NUnit-2.4.6 #{assembly.to_absolute.escape}"
		end
		
		NCover.explore \
			:tool => "#{ncover_path}/NCoverExplorer.console.exe",
			:project => project_name,
			:report_dir => "#{build_dir}/Coverage",
			:html_report => 'CoverageReport.html',
			:min_coverage => 90,
			:fail_if_under_min_coverage => false,
			:statistics => {
				:NCoverCodeCoverage => "/coverageReport/project/@functionCoverage"
			} do |key, value|
				TeamCity.add_statistic key, value
				TeamCity.append_build_status_text "Code coverage: #{Float(value.to_s).round}%"
			end
end