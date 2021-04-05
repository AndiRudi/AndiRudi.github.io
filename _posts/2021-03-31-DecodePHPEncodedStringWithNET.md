---
layout: post
title:  "Decrypt a PHP encrypted text with .NET Core"
author: "Andreas Rudischhauser"
categories: [ Development, .NET Core]
image: 
description: "Decrypt a PHP encrypted text with .NET Core"
---
# Decrypt a PHP encrypted text with .NET Core

I put this here, because it wasn't trivial to figure out the exact code

## PHP Encryptions

```php
   //It seems ctr is not supported in .net so we need to use cbc
   encryptedText = openssl_encrypt(theOriginalText, 'aes-128-cbc', key, 0, substr(iv, 0, 16));
```

### .NET Core Decryption

```cs
    using Aes aes = Aes.Create();

    var key = System.Text.Encoding.UTF8.GetBytes(originalKey.Substring(0, 16));
    var iv = System.Text.Encoding.UTF8.GetBytes(originalIV.Substring(0, 16));
    var encryptedBytes = Convert.FromBase64String(encryptedText); 
    var decryptor = aes.CreateDecryptor(key, iv);
    var decryptedText = "";
    using (var memoryStream = new MemoryStream(encryptedBytes))
    {
        using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
        {
            using (var reader = new StreamReader(cryptoStream))
            {
                decryptedText = reader.ReadToEnd();
            }
        }
    }
```
