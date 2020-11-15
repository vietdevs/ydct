(function () {
    appModule.factory('quanlyhoso.thutuc99.services.appChuKySo', ['$uibModal',
        function ($uibModal) {

            //THU_TUC_ID
            var thuTucId = 99;
            //MA_THU_TUC
            var maThuTuc = app.MA_THU_TUC.THU_TUC_99;

            var xemFilePDF = function (pathPDF, title) {
                var modalInstance = $uibModal.open({
                    templateUrl: '~/App/quanlyhoso/_common/modal/viewFilePDFModal.cshtml',
                    controller: 'quanlyhoso.common.modal.viewFilePDFModal as vm',
                    backdrop: 'static',
                    size: 'lg',
                    resolve: {
                        modalData: {
                            pathPDF: pathPDF,
                            title: title
                        }
                    }
                });

                modalInstance.result.then(function (result) {
                });
            };

            var xemBienBanThamDinh = function (item, fnCallBack) {

                var _pathPDF = "/Report99/TemplateBienBanThamDinh?hoSoId=" + item.id + "#zoom=70";

                var modalInstance = $uibModal.open({
                    templateUrl: '~/App/quanlyhoso/_common/modal/viewFilePDFModal.cshtml',
                    controller: 'quanlyhoso.common.modal.viewFilePDFModal as vm',
                    backdrop: 'static',
                    size: 'lg',
                    resolve: {
                        modalData: {
                            pathPDF: _pathPDF,
                            title: "Biên bản thẩm định",
                            isTemplate: true
                        }
                    }
                });
            };

            var xemTruocBienBanThamDinh = function (item, fnCallBack) {

                var modalInstance = $uibModal.open({
                    templateUrl: '~/App/quanlyhoso/_common/excel/ViewPDF.cshtml',
                    controller: 'quanlyhoso.common.excel.ViewPDF as vm',
                    backdrop: 'static',
                    size: 'lg',
                    resolve: {
                        modalData: {
                            modalTitle: 'Biên bản thẩm định',
                            controller: "Report99",
                            action: "GoToViewBienBanThamDinh",
                            hoSoId: item.id,
                            noiDungThamXetJson: item.noiDungThamXetJson,
                            yKienChung: item.yKienChung
                        }
                    }
                });

                modalInstance.result.then(function (result) {
                    if (fnCallBack) {
                        fnCallBack(item);
                    }
                });
            };

            var xemTruocCongVan = function (item, fnCallBack) {

                var modalInstance = $uibModal.open({
                    templateUrl: '~/App/quanlyhoso/_common/excel/ViewPDF.cshtml',
                    controller: 'quanlyhoso.common.excel.ViewPDF as vm',
                    backdrop: 'static',
                    size: 'lg',
                    resolve: {
                        modalData: {
                            modalTitle: 'Giấy công văn',
                            controller: "Report99",
                            action: "GoToViewCongVan",
                            hoSoId: item.id,
                            noiDungCV: item.noiDungCV,
                            hoSoIsDat: item.hoSoIsDat,
                            noiDungThamXetJson: item.noiDungThamXetJson
                        }
                    }
                });

                modalInstance.result.then(function (result) {
                    if (fnCallBack) {
                        fnCallBack(item);
                    }
                });
            };

            //Ký số
            var kySoHoSo = function (dataItem, fnCallBack) {
                if (dataItem.id > 0) {
                    $.ajax({
                        url: '/Report99/InsertPDFHoSo',
                        type: 'POST',
                        async: false,
                        data: {
                            hoSoId: dataItem.id
                        },
                        success: function (kq) {
                            var data = kq.result;
                            if (data.fileName.length == 0) {
                                return false;
                            }
                            var signedFileName = signUSB(data.fileName);

                            //signedFileName err
                            if (signedFileName.length == 0 || signedFileName == undefined) {
                                alert('Ký điện tử thất bại');
                                return false;
                            }

                            if (fnCallBack) {
                                param = {
                                    hoSoId: dataItem.id,
                                    duongDanTep: signedFileName
                                };

                                fnCallBack(param);
                            }

                            return true;
                        }
                    });
                }
                else {
                    console.log(dataItem, 'dataItem err');
                }
            };

            var kySoCongVan = function (dataItem, fnCallBack) {
                if (dataItem.id > 0) {
                    $.ajax({
                        url: '/Report99/InsertPDFCongVan',
                        type: 'POST',
                        async: false,
                        data: {
                            hoSo: {
                                hoSoId: dataItem.id,
                                hoSoIsDat: dataItem.hoSoIsDat
                            }
                        },
                        success: function (kq) {
                            var data = kq.result;
                            if (data.fileName.length == 0) {
                                return false;
                            }
                            var signedFileName = signUSB(data.fileName);

                            //signedFileName err
                            if (signedFileName.length == 0 || signedFileName == undefined) {
                                alert('Ký điện tử thất bại');
                                return false;
                            }

                            if (fnCallBack) {
                                param = {
                                    hoSoId: dataItem.id,
                                    hoSoXuLyId: dataItem.hoSoXuLyId_Active,
                                    duongDanTep: signedFileName,
                                    soTiepNhan: data.soTiepNhan
                                };

                                fnCallBack(param);
                            }

                            return true;
                        }
                    });
                }
                else {
                    console.log(dataItem, 'dataItem err');
                }
            };

            var kySoGiayTiepNhan = function (dataItem, fnCallBack) {
                if (dataItem.id > 0) {
                    $.ajax({
                        url: '/Report99/InsertPDFCongVan',
                        type: 'POST',
                        async: false,
                        data: {
                            hoSoId: dataItem.id
                        },
                        success: function (kq) {
                            var data = kq.result;
                            if (data.fileName.length == 0) {
                                return false;
                            }
                            var pathCongVanCA = "";
                            var pathGiayTiepNhanCA = "";

                            var signedFileName = signUSB(data.fileName);
                            //signedFileName err
                            if (signedFileName.length == 0 || signedFileName == undefined) {
                                alert('Ký điện tử thất bại');
                                return false;
                            }
                            pathCongVanCA = angular.copy(signedFileName);
                            //ký thành công tạo ra giay tiếp nhận tổng hợp
                            $.ajax({
                                url: '/Report99/InsertPDFGiayTiepNhan',
                                type: 'POST',
                                async: false,
                                data: {
                                    hoSoId: dataItem.id
                                },
                                success: function (kq) {
                                    var data = kq.result;
                                    if (data.fileName.length == 0) {
                                        return false;
                                    }
                                    var signedFileName = data.fileName;
                                    var soTiepNhan = data.soTiepNhan;

                                    signedFileName = signUSB(signedFileName);
                                    if (signedFileName.length == 0 || signedFileName == undefined) {
                                        alert('Ký điện tử thất bại');
                                        return false;
                                    }
                                    pathGiayTiepNhanCA = angular.copy(signedFileName);

                                    if (fnCallBack) {
                                        param = {
                                            hoSoId: dataItem.id,
                                            hoSoXuLyId: dataItem.hoSoXuLyId_Active,
                                            duongDanTep: pathCongVanCA,
                                            giayTiepNhanCA: pathGiayTiepNhanCA,
                                            soTiepNhan: soTiepNhan
                                        };

                                        fnCallBack(param);
                                    }

                                    return true;
                                }
                            });

                            return false;
                        }
                    });
                }
                else {
                    console.log(dataItem, 'dataItem err');
                }
            };

            var vanThuDongDau = function (dataItem, fnCallBack) {

                if (dataItem.id > 0) {
                    $.ajax({
                        url: '/Report99/GetDuongDanTepGiayCongVan',
                        type: 'POST',
                        async: false,
                        data: {
                            hoSoId: dataItem.id
                        },
                        success: function (kq) {
                            var data = kq.result;
                            if (data.status == 1) {

                                //Hồ sơ đạt => giấy tiếp nhận
                                //if (dataItem.trangThaiCV == 1) {
                                //    if (data.fileName.length == 0 && data.giayTiepNhanCA.length == 0) {
                                //        return false;
                                //    }


                                //    var urlGiayTiepNhan = angular.copy(data.fileName);
                                //    var urlGiayTiepNhanFull = angular.copy(data.giayTiepNhanCA);

                                //    //Giấy tiếp nhận | Công văn
                                //    var giayTiepNhanCA = signUSB(urlGiayTiepNhan);
                                //    //signedFileName err
                                //    if (giayTiepNhanCA == "" || giayTiepNhanCA.length == 0 || giayTiepNhanCA == undefined) {
                                //        alert('Ký điện tử thất bại');
                                //        return false;
                                //    }

                                //    //Giấy tiếp nhận đầy đủ
                                //    var giayTiepNhanFullCA = signUSB(urlGiayTiepNhanFull);
                                //    if (giayTiepNhanFullCA == "" || giayTiepNhanFullCA.length == 0 || giayTiepNhanFullCA == undefined) {
                                //        alert('Ký điện tử thất bại');
                                //        return false;
                                //    }

                                //    if (fnCallBack) {
                                //        var param = {
                                //            duongDanTep: giayTiepNhanCA,
                                //            giayTiepNhanCA: giayTiepNhanFullCA
                                //        }

                                //        fnCallBack(param);
                                //    }

                                //}
                                ////Hồ sơ không đạt => đóng dấu công văn
                                //else {
                                if (data.fileName.length == 0) {
                                    return false;
                                }

                                var signedFileName = signUSB(data.fileName);

                                //signedFileName err
                                if (signedFileName.length == 0 || signedFileName == undefined) {
                                    alert('Ký điện tử thất bại');
                                    return false;
                                }

                                if (fnCallBack) {
                                    var param = {
                                        duongDanTep: signedFileName
                                    };

                                    fnCallBack(param);
                                }
                                //}
                                return true;
                            }

                        }
                    });
                }
                else {
                    console.log(dataItem, 'dataItem err');
                }
            };
            //End - Ký số

            return {
                THU_TUC_ID: thuTucId,
                MA_THU_TUC: maThuTuc,
                xemFilePDF,
                xemTruocCongVan,
                xemBienBanThamDinh,
                xemTruocBienBanThamDinh,
                kySoHoSo,
                kySoCongVan,
                kySoGiayTiepNhan,
                vanThuDongDau
            };
        }
    ]);
})();