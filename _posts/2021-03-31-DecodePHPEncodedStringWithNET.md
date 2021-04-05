---
layout: post-sidebar
date: 2021-03-31
title: "Decrypt PHP encrypted strings with .net core"
categories: coding
author_name : Andi
author_url : /author/andi
author_avatar: andi
show_avatar : true
read_time : 22
feature_image: feature-water
show_related_posts: true
square_related: recommend-spain
---

I once needed to decrypt a php encoded string and it turned out to be more complex than anticipated. This is just a gist.

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
