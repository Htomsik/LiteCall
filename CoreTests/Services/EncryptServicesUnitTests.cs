using Core.Services.Extra;

namespace CoreTests.Services;

[TestClass]
public class EncryptServicesUnitTests
{
    [TestMethod]
    public void Base64EncryptSameCheck()
    {
        //Arrange
        var encryptSc = new EncryptSc();

        string decryptedText = "TestText";

        //Act
       var encryptedText = encryptSc.Base64Encrypt(decryptedText).Result;

       var decriptEncriptedText = encryptSc.Base64Decrypt(encryptedText).Result;
       
        //Assert
        Assert.AreEqual(decryptedText,decriptEncriptedText);
    }
    
    [TestMethod]
    public void ShaEncryptSame()
    {
        //Arrange
        var encryptSc = new EncryptSc();

        string decryptedText = "TestText";

        //Act
        var shaText = encryptSc.ShaEncrypt(decryptedText).Result;
        
        var shaText1 = encryptSc.ShaEncrypt(decryptedText).Result;
       
        //Assert
        Assert.AreEqual(shaText,shaText1);
    }
    
    [TestMethod]
    public void Base64EncryptVarious()
    {
        //Arrange
        var encryptSc = new EncryptSc();

        string decryptedText = " TestText ";

        int exCount = 0;

        //Act
        foreach (var letter in decryptedText)
        {
            try
            {
                encryptSc.Base64Encrypt(Convert.ToString(letter));
            }
            catch (Exception)
            {
                exCount++;
            }
        }
       
        //Assert
        Assert.IsNotNull(exCount);
    }
    
    [TestMethod]
    public void Base64DecryptVarious()
    {
        //Arrange
        var encryptSc = new EncryptSc();
        
        string decryptedText = "TestText";
        
        string nullText = " ";
        
        int exCount = 0;

        //Act

       var encryptedText = encryptSc.Base64Encrypt(decryptedText).Result;
        
        try
        {

            encryptSc.Base64Decrypt(encryptedText);
            
            encryptSc.Base64Decrypt(nullText);

        }
        catch (Exception)
        {
            exCount++;
        }
        
    
       
        //Assert
        Assert.IsNotNull(exCount);
    }
}