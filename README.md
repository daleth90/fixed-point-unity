# Fixed Point
![GitHub package.json version](https://img.shields.io/github/package-json/v/daleth90/fixed-point-unity)
![GitHub last commit](https://img.shields.io/github/last-commit/daleth90/fixed-point-unity)
![GitHub](https://img.shields.io/github/license/daleth90/fixed-point-unity)

Use Q31.32 format to represent fixed-point numbers.  
This library is largely referenced from [asik/FixedMath.Net](https://github.com/asik/FixedMath.Net),
but fix and adjust some API for Unity usage.

Considered Q15.16 format before, but the resolution is only 2^-16 = 1.52587897E-5  
This mean that it only provides 4 decimals of accuracy, which is unacceptable in many cases.
