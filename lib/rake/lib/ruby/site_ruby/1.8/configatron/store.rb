class Configatron
  class Store
    alias_method :send!, :send
    
    # Takes an optional Hash of parameters
    def initialize(options = {}, name = nil, parent = nil)
      @_name = name
      @_parent = parent
      @_store = {}
      configure_from_hash(options)
      @_protected = []
    end
    
    # Returns a Hash representing the configurations
    def to_hash
      @_store
    end
    
    def heirarchy
      path = [@_name]
      parent = @_parent
      until parent.nil?
        path << parent.instance_variable_get('@_name')
        parent = parent.instance_variable_get('@_parent')
      end
      path.compact!
      path.reverse!
      path.join('.')
    end
    
    def configatron_keys
      return @_store.keys.collect{|k| k.to_s}.sort
    end
    
    # Checks whether or not a parameter exists
    # 
    # Examples:
    #   configatron.i.am.alive = 'alive!'
    #   configatron.i.am.exists?(:alive) # => true
    #   configatron.i.am.exists?(:dead) # => false
    def exists?(name)
      @_store.has_key?(name.to_sym) || @_store.has_key?(name.to_s)
    end
    
    def inspect
      path = [@_name]
      parent = @_parent
      until parent.nil?
        path << parent.instance_variable_get('@_name')
        parent = parent.instance_variable_get('@_parent')
      end
      path << 'configatron'
      path.compact!
      path.reverse!
      f_out = []
      @_store.each do |k, v|
        if v.is_a?(Configatron::Store)
          v.inspect.each_line do |line|
            if line.match(/\n/)
              line.each_line do |l|
                l.strip!
                f_out << l
              end
            else
              line.strip!
              f_out << line
            end
          end
        else
          f_out << "#{path.join('.')}.#{k} = #{v.inspect}"
        end
      end
      f_out.compact.sort.join("\n")
    end

    # Allows for the configuration of the system via a Hash
    def configure_from_hash(options)
      parse_options(options)
    end

    # Allows for the configuration of the system from a YAML file.
    # Takes the path to the YAML file.  Also takes an optional parameter,
    # <tt>:hash</tt>, that indicates a specific hash that should be
    # loaded from the file. 
    def configure_from_yaml(path, opts = {})
      begin
        yml = ::Yamler.load(path)
        yml = yml[opts[:hash]] unless opts[:hash].nil?
        configure_from_hash(yml)
      rescue Errno::ENOENT => e
        puts e.message
      end
    end

    # Returns true if there are no configuration parameters
    def nil?
      return @_store.empty?
    end

    # Retrieves a certain parameter and if that parameter
    # doesn't exist it will return the default_value specified.
    def retrieve(name, default_value = nil)
      @_store[name.to_sym] || default_value
    end
    
    # Removes a parameter. In the case of a nested parameter
    # it will remove all below it.
    def remove(name)
      @_store.delete(name.to_sym)
    end

    # Sets a 'default' value. If there is already a value specified
    # it won't set the value.
    def set_default(name, default_value)
      unless @_store[name.to_sym]
        @_store[name.to_sym] = parse_options(default_value)
      end
    end
    
    def method_missing(sym, *args) # :nodoc:
      if sym.to_s.match(/(.+)=$/)
        name = sym.to_s.gsub("=", '').to_sym
        raise Configatron::ProtectedParameter.new(name) if @_protected.include?(name) || methods_include?(name)
        raise Configatron::LockedNamespace.new(@_name) if @_locked && !@_store.has_key?(name)
        @_store[name] = parse_options(*args)
      elsif @_store.has_key?(sym)
        return @_store[sym]
      else
        store = Configatron::Store.new({}, sym, self)
        @_store[sym] = store
        return store
      end
    end
    
    def ==(other) # :nodoc:
      self.to_hash == other
    end
    
    # Prevents a parameter from being reassigned. If called on a 'namespace' then
    # all parameters below it will be protected as well.
    def protect(name)
      @_protected << name.to_sym
    end

    # Prevents all parameters from being reassigned.
    def protect_all!
      @_protected.clear
      @_store.keys.each do |k|
        val = self.send(k)
        val.protect_all! if val.class == Configatron::Store
        @_protected << k
      end
    end
    
    # Removes the protection of a parameter.
    def unprotect(name)
      @_protected.reject! { |e| e == name.to_sym }
    end
    
    def unprotect_all!
      @_protected.clear
      @_store.keys.each do |k|
        val = self.send(k)
        val.unprotect_all! if val.class == Configatron::Store
      end
    end

    # Prevents a namespace from having new parameters set. The lock is applied
    # recursively to any namespaces below it.
    def lock(name)
      namespace = @_store[name.to_sym]
      raise ArgumentError, "Namespace #{name.inspect} does not exist" if namespace.nil?
      namespace.lock!
    end

    def unlock(name)
      namespace = @_store[name.to_sym]
      raise ArgumentError, "Namespace #{name.inspect} does not exist" if namespace.nil?
      namespace.unlock!
    end
    
    # = DeepClone
    #
    # == Version
    #  1.2006.05.23 (change of the first number means Big Change)
    #
    # == Description
    #  Adds deep_clone method to an object which produces deep copy of it. It means
    #  if you clone a Hash, every nested items and their nested items will be cloned.
    #  Moreover deep_clone checks if the object is already cloned to prevent endless recursion.
    #
    # == Usage
    #
    #  (see examples directory under the ruby gems root directory)
    #
    #   require 'rubygems'
    #   require 'deep_clone'
    #
    #   include DeepClone
    #
    #   obj = []
    #   a = [ true, false, obj ]
    #   b = a.deep_clone
    #   obj.push( 'foo' )
    #   p obj   # >> [ 'foo' ]
    #   p b[2]  # >> []
    #
    # == Source
    # http://simplypowerful.1984.cz/goodlibs/1.2006.05.23
    #
    # == Author
    #  jan molic (/mig/at_sign/1984/dot/cz/)
    #
    # == Licence
    #  You can redistribute it and/or modify it under the same terms of Ruby's license;
    #  either the dual license version in 2003, or any later version.
    #
    def deep_clone( obj=self, cloned={} )
      if cloned.has_key?( obj.object_id )
        return cloned[obj.object_id]
      else
        begin
          cl = obj.clone
        rescue Exception
          # unclonnable (TrueClass, Fixnum, ...)
          cloned[obj.object_id] = obj
          return obj
        else
          cloned[obj.object_id] = cl
          cloned[cl.object_id] = cl
          if cl.is_a?( Hash )
            cl.clone.each { |k,v|
              cl[k] = deep_clone( v, cloned )
            }
          elsif cl.is_a?( Array )
            cl.collect! { |v|
              deep_clone( v, cloned )
            }
          end
          cl.instance_variables.each do |var|
            v = cl.instance_eval( var.to_s )
            v_cl = deep_clone( v, cloned )
            cl.instance_eval( "#{var} = v_cl" )
          end
          return cl
        end
      end
    end
    
    protected
    def lock!
      @_locked = true
      @_store.values.each { |store| store.lock! if store.is_a?(Configatron::Store) }
    end

    def unlock!
      @_locked = false
      @_store.values.each { |store| store.unlock! if store.is_a?(Configatron::Store) }
    end
    
    private
    def methods_include?(name)
      self.methods.include?(RUBY_VERSION > '1.9.0' ? name.to_sym : name.to_s)
    end
    
    def parse_options(options)
      if options.is_a?(Hash)
        options.each do |k,v|
          if v.is_a?(Hash)
            self.method_missing(k.to_sym).configure_from_hash(v)
          else
            self.method_missing("#{k.to_sym}=", v)
          end
        end
      else
        return options
      end
    end
    
    undef :test # :nodoc:
    
  end # Store
end # Configatron
