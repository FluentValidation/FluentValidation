require 'erb'
require 'yaml'
require 'fileutils'

Dir.glob(File.join(File.dirname(__FILE__), 'yamler', '**/*.rb')).each do |f|
  require f
end