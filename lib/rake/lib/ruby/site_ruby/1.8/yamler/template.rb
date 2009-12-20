module Yamler
  
  class Template
    
    # The path of the YAML file to be rendered
    attr_accessor :path
    # Options that are available to the YAML file.
    attr_accessor :options
    
    # Takes the path to the YAML file you wish to render.
    # An optional <tt>Hash</tt> of options can be passed in.
    # These options are available via the <tt>options</tt> accessor.
    # If there is a <tt>Hash</tt> in the <tt>options</tt> called
    # <tt>:locals</tt> then the keys of that <tt>Hash are available</tt>
    # as local methods.
    # 
    # Examples:
    #   Yamler::Template.new('/path/to/file.yml', {:locals => {:username => 'markbates'}, :foo => :bar})
    #   
    #   # in file.yml:
    #   username: <%= username %> # => 'markbates'
    #   foo: <%= options[:foo] %> # => :bar
    def initialize(path, options = {})
      self.path = File.expand_path(path)
      self.options = options
    end
    
    # Runs the YAML file through ERB using either the current
    # templates <tt>binding</tt> or the specified one. This
    # method returns a string and <i>NOT</i> a YAML object.
    def render(b = binding)
      res = ERB.new(File.read(self.path)).result(b)
      res
    end
    
    def method_missing(sym, *args) # :nodoc:
      raise NoMethodError.new(sym.to_s) if self.options[:locals].nil? || self.options[:locals][sym].nil?
      return self.options[:locals][sym]
    end
    
    # Requires another YAML file from inside the current YAML file. The contents
    # of the required YAML file will be run through ERB with the binding of the
    # requiring YAML file and it's output will be appended to the calling YAML file.
    # The '.yml' extension is optional. It will be added on if the extension
    # is blank. If the file does not exist, it will look for it in the current
    # directory. If it does not exist there it will raise an error.
    # 
    # Examples:
    #   <%= require_yaml('foo') %> # => <current_yml_files_directory>/foo.yml
    #   <%= require_yaml('foo.yml') %> # => <current_yml_files_directory>/foo.yml
    #   <%= require_yaml('/usr/foo.yml') %> # => /usr/foo.yml
    def require_yaml(path)
      path = File.extname(path) == '' ? "#{path}.yml" : path
      unless File.exists?(path)
        path = File.expand_path(File.join(File.dirname(self.path), path))
      end
      Yamler::Template.new(path).render(binding)
    end
    
    # Returns the path of the current YAML file.
    def __FILE__
      self.path
    end
    
  end
  
end