/**
 * @license Copyright (c) 2003-2014, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.md or http://ckeditor.com/license
 */

CKEDITOR.editorConfig = function (config) {
    // Define changes to default configuration here. For example:
    // config.language = 'fr';
    // config.uiColor = '#AADC6E';
    config.language = 'vi';
    config.enterMode = CKEDITOR.ENTER_BR;//không tạo ra thẻ p khi nhập.
    //var roxyFileman = '/Content/fileman/index.html';
    //config.filebrowserBrowseUrl = roxyFileman,
    //config.filebrowserImageBrowseUrl = roxyFileman + '?type=image',
    //config.removeDialogTabs = 'link:upload;image:upload'
    config.filebrowserBrowseUrl = '/libs/ckfinder/ckfinder.html';
    config.filebrowserImageBrowseUrl = '/libs/ckfinder/ckfinder.html?Type=Images';
    config.filebrowserFlashBrowseUrl = '/libs/ckfinder/ckfinder.html?Type=Flash';
    config.filebrowserUploadUrl = '/libs/ckfinder/core/connector/aspx/connector.aspx?command=QuickUpload&type=Files';
    config.filebrowserImageUploadUrl = '/libs/ckfinder/core/connector/aspx/connector.aspx?command=QuickUpload&type=Images';
    config.filebrowserFlashUploadUrl = '/libs/ckfinder/core/connector/aspx/connector.aspx?command=QuickUpload&type=Flash';
    config.allowedContent = true;
    config.extraAllowedContent = 'p(*)[*]{*};div(*)[*]{*};li(*)[*]{*};ul(*)[*]{*}';
};
CKFinder.setupCKEditor(null, '/libs/ckfinder/');
CKEDITOR.dtd.$removeEmpty.i = 0;