/* 
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
var VtPlugin = {
    
initPlugin : function()
{

    return document.getElementById('plugin0').getVersion == null ? false : true;
},
getVersion : function()
{
    return document.getElementById('plugin0').getVersion();
},
getFileName : function()
{
    return document.getElementById('plugin0').getFileName();
},
getCert : function()
{
    return document.getElementById('plugin0').getCert();
},
getCertChain : function()
{
    return document.getElementById('plugin0').getCertChain();
},
signHash : function(base64Hash, certSerial)
{
    return document.getElementById('plugin0').signHash(base64Hash, certSerial);
},
signPdf : function(filePath, reason, location)
{
    return document.getElementById('plugin0').signPdf(filePath, reason, location);
},
signPdf : function(filePath, reason, location, fileDest)
{
    return document.getElementById('plugin0').signPdf(filePath, reason, location, fileDest);
},
signOOXml : function(filePath)
{
    return document.getElementById('plugin0').signOOXml(filePath);
},
signOOXml : function(filePath, fileDest)
{
    return document.getElementById('plugin0').signOOXml(filePath, fileDest);
},
signXml : function(filePath)
{
    return document.getElementById('plugin0').signOOXml(filePath);
},
signXml : function(filePath, fileDest)
{
    return document.getElementById('plugin0').signXml(filePath, fileDest);
}

};  












/**
 * 
 * @param {type} base64Hash
 * @param {type} certSerial
 * @returns {@exp;@call;plugin@call;signHash}
 */



