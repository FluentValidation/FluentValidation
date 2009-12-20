class String # :nodoc:
  
  def to_configatron(*args)
    name_spaces = (args + self.split("::")).flatten
    name_spaces.collect!{|s| s.to_s.methodize}
    configatron.send_with_chain(name_spaces)
  end
  
  def underscore # :nodoc:
    self.to_s.gsub(/::/, '/').
      gsub(/([A-Z]+)([A-Z][a-z])/,'\1_\2').
      gsub(/([a-z\d])([A-Z])/,'\1_\2').
      tr("-", "_").
      downcase
  end
  
  def methodize # :nodoc:
    x = self
    
    # if we get down to a nil or an empty string raise an exception! 
    raise NameError.new("#{self} cannot be converted to a valid method name!") if x.nil? || x == ''
    
    # get rid of the big stuff in the front/back
    x.strip!
    
    # if we get down to a nil or an empty string raise an exception! 
    raise NameError.new("#{self} cannot be converted to a valid method name!") if x.nil? || x == ''
    
    x = x.underscore
    
    # get rid of spaces and make the _
    x.gsub!(' ', '_')
    # get rid of everything that isn't 'safe' a-z, 0-9, ?, !, =, _
    x.gsub!(/([^ a-zA-Z0-9\_\?\!\=]+)/n, '_')
    
    # if we get down to a nil or an empty string raise an exception! 
    raise NameError.new("#{self} cannot be converted to a valid method name!") if x.nil? || x == ''
    
    # condense multiple 'safe' non a-z chars to just one.
    # ie. ___ becomes _ !!!! becomes ! etc...
    [' ', '_', '?', '!', "="].each do |c|
      x.squeeze!(c)
    end
    
    # if we get down to a nil or an empty string raise an exception! 
    raise NameError.new("#{self} cannot be converted to a valid method name!") if x.nil? || x == ''
    
    #down case the whole thing
    x.downcase!
    
    # get rid of any characters at the beginning that aren't a-z
    while !x.match(/^[a-z]/)
      x.slice!(0)
      
      # if we get down to a nil or an empty string raise an exception! 
      raise NameError.new("#{self} cannot be converted to a valid method name!") if x.nil? || x == ''
    end
    
    # let's trim this bad boy down a bit now that we've cleaned it up, somewhat.
    # we should do this before cleaning up the end character, because it's possible to end up with a 
    # bad char at the end if you trim too late.
    x = x[0..100] if x.length > 100
    
    # get rid of any characters at the end that aren't safe
    while !x.match(/[a-z0-9\?\!\=]$/)
      x.slice!(x.length - 1)
      # if we get down to a nil or an empty string raise an exception! 
      raise NameError.new("#{self} cannot be converted to a valid method name!") if x.nil? || x == ''
    end
    
    # if we get down to a nil or an empty string raise an exception! 
    raise NameError.new("#{self} cannot be converted to a valid method name!") if x.nil? || x == ''
    
    # let's get rid of characters that don't belong in the 'middle' of the method.
    orig_middle = x[1..(x.length - 2)]
    n_middle = orig_middle.dup
    
    ['?', '!', "="].each do |c|
      n_middle.gsub!(c, "_")
    end
    
    # the previous gsub can leave us with multiple underscores that need cleaning up.
    n_middle.squeeze!("_")
    
    x.gsub!(orig_middle, n_middle)
    x.gsub!("_=", "=")
    x
  end
  
end