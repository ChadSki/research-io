
setSlot("fac", method(n, if(n == (0), 1, n * (fac(n - (1))))))
setSlot("linearAsyncBlock", method(1 println; yield; 2 println; yield; 3 println; 42))
setSlot("yieldAll", method(while (yieldingCoros>(0), yield)))
