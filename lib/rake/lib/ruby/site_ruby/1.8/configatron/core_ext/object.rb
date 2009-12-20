class Object # :nodoc:
  
  def send_with_chain(methods, *args) # :nodoc:
    obj = self
    [methods].flatten.each {|m| obj = obj.send(m, *args)}
    obj
  end
  
end