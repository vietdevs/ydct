(function () {
    appModule.directive('app.directives.dirdanhsachthutucdashboard', ['$compile', '$timeout', '$templateRequest', '$filter', 'appSession', '$linq', 
        function ($compile, $timeout, $templateRequest, $filter, appSession, $linq) {
            var controller = ['$rootScope', '$scope', '$timeout', '$window', '$filter', '$uibModal', '$state',
                function ($rootScope, $scope, $timeout, $window, $filter, $uibModal, $state) {
                    var vm = this;
                    vm.menu = angular.copy(abp.nav.menus.MainMenu);
                    vm.thuTucs = [];
                    vm.arrThuTuc = [];
                    vm.showActive = false;
                    vm.hoSoMoiCount = 0;
                    vm.hoSoQuaHan = 0;
                    vm.hoSoToiHan = 0;
                    vm.listQuaHan = [];
                    vm.listToiHan = [];
                    var initVar = () => {
                        vm.ROLE_LEVEL = app.ROLE_LEVEL;
                        vm.session = appSession;
                        vm.roleLv = appSession.user.roleLevel;
                        vm.isAdmin = (appSession.user.userName == 'admin' || vm.roleLv == vm.ROLE_LEVEL.SA);
                        vm.showAll = (vm.roleLv == app.ROLE_LEVEL.DOANH_NGHIEP || vm.isAdmin == true);
                        vm.toggleShowAll = function () {
                            vm.showAll = !vm.showAll;
                            app.showAllThuTuc = vm.showAll;
                        };
                        vm.filter = {
                            trangThaiQuaHan: 0
                        };
                    };

                    var getThaoTacThuTucFromMenu = (thuTucEnum) => {
                        try {
                            _thuTucMenu = $linq.Enumerable().From(vm.menu.items)
                                .Where(function (x) {
                                    return x.displayName == app.keyThuTuc
                                }).FirstOrDefault();
                            if (_thuTucMenu && _thuTucMenu.items) {
                                _thaoTacMenu = $linq.Enumerable().From(_thuTucMenu.items)
                                    .Where(function (x) {
                                        return ((x.displayName != 'Danh sách thủ tục' && x.displayName != 'Danh mục thủ tục') || x.items[0]) && x.items[0].url.indexOf(thuTucEnum) > -1
                                    }).FirstOrDefault();
                                return _thaoTacMenu.items;
                            }

                            return [];
                        }
                        catch (ex) {
                            return [];
                        }
                    }

                    var getRequestTotalThuTuc = () => {
                        angular.forEach(vm.thuTucs, function (thuTuc) {
                            let _thutucEnum = thuTuc.thuTucIdEnum < 9 ? '0' + angular.copy(thuTuc.thuTucIdEnum) : angular.copy(thuTuc.thuTucIdEnum);
                            thuTuc.thaotacthutuc = getThaoTacThuTucFromMenu(_thutucEnum);
                            thuTuc.totalXuLy = 0;
                            angular.forEach(thuTuc.thaotacthutuc, function (thaoTac) {

                                //TT03 custom
                                if (thaoTac.url.indexOf('tt37/rasoathoso') != -1) {
                                    _ret = {
                                        thuTuc: thuTuc.thuTucIdEnum,
                                        formId: app.FORM_ID.FORM_MOT_CUA_RA_SOAT,
                                        listFormCase: [1]
                                    }
                                    abp.services.app.thuTucCommon.getCountThuTucDashBoard(_ret).then(function (ret) {
                                        thaoTac.total = ret.total;
                                        thuTuc.totalXuLy += ret.total;
                                        $scope.$apply();
                                    });
                                }

                                else if (thaoTac.url.indexOf('tt37/motcuaphancong') != -1) {
                                    _ret = {
                                        thuTuc: thuTuc.thuTucIdEnum,
                                        formId: app.FORM_ID.FORM_MOT_CUA_PHAN_CONG,
                                        listFormCase: [1]
                                    }
                                    abp.services.app.thuTucCommon.getCountThuTucDashBoard(_ret).then(function (ret) {
                                        thaoTac.total = ret.total;
                                        thuTuc.totalXuLy += ret.total;
                                        $scope.$apply();
                                    });
                                }
                                else if (thaoTac.url.indexOf('tt37/phongbanphancong') != -1) {
                                    _ret = {
                                        thuTuc: thuTuc.thuTucIdEnum,
                                        formId: app.FORM_ID.FORM_PHONG_BAN_PHAN_CONG,
                                        listFormCase: [1]
                                    }
                                    abp.services.app.thuTucCommon.getCountThuTucDashBoard(_ret).then(function (ret) {
                                        thaoTac.total = ret.total;
                                        thuTuc.totalXuLy += ret.total;
                                        $scope.$apply();
                                    });
                                }
                                else if (thaoTac.url.indexOf('tt37/thamxethoso') != -1) {
                                    _ret = {
                                        thuTuc: thuTuc.thuTucIdEnum,
                                        formId: app.FORM_ID.FORM_THAM_XET_HO_SO,
                                        listFormCase: [1,4]
                                    }
                                    abp.services.app.thuTucCommon.getCountThuTucDashBoard(_ret).then(function (ret) {
                                        thaoTac.total = ret.total;
                                        thuTuc.totalXuLy += ret.total;
                                        $scope.$apply();
                                    });
                                }
                                else if (thaoTac.url.indexOf('tt37/thamdinhhoso') != -1) {
                                    _ret = {
                                        thuTuc: thuTuc.thuTucIdEnum,
                                        formId: 371,// thẩm định hồ sơ 37
                                        listFormCase: [1]
                                    }
                                    abp.services.app.thuTucCommon.getCountThuTucDashBoard(_ret).then(function (ret) {
                                        thaoTac.total = ret.total;
                                        thuTuc.totalXuLy += ret.total;
                                        $scope.$apply();
                                    });
                                }
                                else if (thaoTac.url.indexOf('tt37/tonghopthamdinh') != -1) {
                                    _ret = {
                                        thuTuc: thuTuc.thuTucIdEnum,
                                        formId: 372,
                                        listFormCase: [1, 2, 3]
                                    }
                                    abp.services.app.thuTucCommon.getCountThuTucDashBoard(_ret).then(function (ret) {
                                        thaoTac.total = ret.total;
                                        thuTuc.totalXuLy += ret.total;
                                        $scope.$apply();
                                    });
                                }
                                else if (thaoTac.url.indexOf('tt37/thamdinhtruongphongduyet') != -1) {
                                    _ret = {
                                        thuTuc: thuTuc.thuTucIdEnum,
                                        formId: 373,
                                        listFormCase: [1, 2]
                                    }
                                    abp.services.app.thuTucCommon.getCountThuTucDashBoard(_ret).then(function (ret) {
                                        thaoTac.total = ret.total;
                                        thuTuc.totalXuLy += ret.total;
                                        $scope.$apply();
                                    });
                                }
                                else if (thaoTac.url.indexOf('tt37/thamdinhlanhdaocucduyet') != -1) {
                                    _ret = {
                                        thuTuc: thuTuc.thuTucIdEnum,
                                        formId: 374,
                                        listFormCase: [1]
                                    }
                                    abp.services.app.thuTucCommon.getCountThuTucDashBoard(_ret).then(function (ret) {
                                        thaoTac.total = ret.total;
                                        thuTuc.totalXuLy += ret.total;
                                        $scope.$apply();
                                    });
                                }
                                else if (thaoTac.url.indexOf('tt37/thamdinhvanthuduyet') != -1) {
                                    _ret = {
                                        thuTuc: thuTuc.thuTucIdEnum,
                                        formId: 375,
                                        listFormCase: [1]
                                    }
                                    abp.services.app.thuTucCommon.getCountThuTucDashBoard(_ret).then(function (ret) {
                                        thaoTac.total = ret.total;
                                        thuTuc.totalXuLy += ret.total;
                                        $scope.$apply();
                                    });
                                }

                                //dùng Chung
                                else if (thaoTac.url && thaoTac.url.indexOf('/dangkyhoso') != -1) {
                                    _ret = {
                                        thuTuc: thuTuc.thuTucIdEnum,
                                        formId: app.FORM_ID.FORM_DANG_KY_HO_SO
                                    }
                                    abp.services.app.thuTucCommon.getCountFormCaseThuTucSql(_ret).then(function (ret) {
                                        thaoTac.total = ret.case1;
                                        thuTuc.totalXuLy += ret.case1;
                                        thaoTac.total += ret.case3;
                                        thuTuc.totalXuLy += ret.case3;
                                        thaoTac.total += ret.case5;
                                        thuTuc.totalXuLy += ret.case5;
                                        thaoTac.total += ret.case6;
                                        thuTuc.totalXuLy += ret.case6;
                                        thaoTac.total += ret.case8;
                                        thuTuc.totalXuLy += ret.case8;
                                        $scope.$apply();
                                    });
                                }
                                else if (thaoTac.url.indexOf('/xacnhanthanhtoan') != -1) {
                                    _ret = {
                                        thuTuc: thuTuc.thuTucIdEnum,
                                        formId: app.FORM_ID.FORM_KE_TOAN_XAC_NHAN_THANH_TOAN
                                    }
                                    abp.services.app.thuTucCommon.getCountFormCaseThuTucSqlKeToan(_ret).then(function (ret) {
                                        thuTuc.totalXuLy = ret.case1;
                                        thaoTac.total = ret.case1;
                                        $scope.$apply();
                                    });
                                }
                                else if (thaoTac.url.indexOf('/rasoathoso') != -1) {
                                    _ret = {
                                        thuTuc: thuTuc.thuTucIdEnum,
                                        formId: app.FORM_ID.FORM_MOT_CUA_RA_SOAT
                                    }
                                    abp.services.app.thuTucCommon.getCountFormCaseThuTucSql(_ret).then(function (ret) {
                                        thaoTac.total = ret.case1;
                                        thuTuc.totalXuLy += ret.case1;
                                        thaoTac.total += ret.case4;
                                        thuTuc.totalXuLy += ret.case4;

                                        thuTuc.totalXuLy += ret.case1;
                                        $scope.$apply();
                                    });
                                }
                                else if (thaoTac.url.indexOf('/motcuaphancong') != -1) {
                                    _ret = {
                                        thuTuc: thuTuc.thuTucIdEnum,
                                        formId: app.FORM_ID.FORM_MOT_CUA_PHAN_CONG
                                    }
                                    abp.services.app.thuTucCommon.getCountFormCaseThuTucSql(_ret).then(function (ret) {
                                        thaoTac.total = ret.case1;
                                        thuTuc.totalXuLy += ret.case1;
                                        $scope.$apply();
                                    });
                                }
                                else if (thaoTac.url.indexOf('/phongbanphancong') != -1) {
                                    _ret = {
                                        thuTuc: thuTuc.thuTucIdEnum,
                                        formId: app.FORM_ID.FORM_PHONG_BAN_PHAN_CONG
                                    }
                                    abp.services.app.thuTucCommon.getCountFormCaseThuTucSql(_ret).then(function (ret) {
                                        thaoTac.total = ret.case1;
                                        thuTuc.totalXuLy += ret.case1;
                                        $scope.$apply();
                                    });
                                }
                                else if (thaoTac.url.indexOf('/thamdinhhoso') != -1) {
                                    _ret = {
                                        thuTuc: thuTuc.thuTucIdEnum,
                                        formId: app.FORM_ID.FORM_THAM_XET_HO_SO,
                                        listFormCase: [1, 3, 4]//FORM_CASE_THAM_XET_HO_SO
                                    }
                                    abp.services.app.thuTucCommon.getCountThuTucDashBoard(_ret).then(function (ret) {
                                        thaoTac.total = ret.total;
                                        thuTuc.totalXuLy += ret.total;
                                        $scope.$apply();
                                    });
                                }
                                else if (thaoTac.url.indexOf('/phophongduyet') != -1) {
                                    _ret = {
                                        thuTuc: thuTuc.thuTucIdEnum,
                                        formId: app.FORM_ID.FORM_PHO_PHONG_DUYET
                                    }
                                    abp.services.app.thuTucCommon.getCountFormCaseThuTucSql(_ret).then(function (ret) {
                                        thaoTac.total = ret.case1;

                                        thuTuc.totalXuLy += ret.case1;
                                        $scope.$apply();
                                    });
                                }
                                else if (thaoTac.url.indexOf('/truongphongduyet') != -1) {
                                    _ret = {
                                        thuTuc: thuTuc.thuTucIdEnum,
                                        formId: app.FORM_ID.FORM_TRUONG_PHONG_DUYET,
                                        listFormCase: [1, 2]//FORM_CASE_TRUONG_PHONG_DUYET
                                    }
                                    abp.services.app.thuTucCommon.getCountThuTucDashBoard(_ret).then(function (ret) {
                                        thaoTac.total = ret.total;
                                        thuTuc.totalXuLy += ret.total;
                                        $scope.$apply();
                                    });
                                }
                                else if (thaoTac.url.indexOf('/lanhdaocucduyet') != -1) {
                                    _ret = {
                                        thuTuc: thuTuc.thuTucIdEnum,
                                        formId: app.FORM_ID.FORM_LANH_DAO_CUC_DUYET,
                                        listFormCase: [1]//FORM_CASE_LANH_DAO_CUC_DUYET
                                    }
                                    abp.services.app.thuTucCommon.getCountThuTucDashBoard(_ret).then(function (ret) {
                                        thaoTac.total = ret.total;
                                        thuTuc.totalXuLy += ret.total;
                                        $scope.$apply();
                                    });
                                }
                                else if (thaoTac.url.indexOf('/lanhdaocucduyetcongvan') != -1) {
                                    _ret = {
                                        thuTuc: thuTuc.thuTucIdEnum,
                                        formId: app.FORM_ID.FORM_LANH_DAO_CUC_DUYET_CONG_VAN
                                    }
                                    abp.services.app.thuTucCommon.getCountFormCaseThuTucSql(_ret).then(function (ret) {
                                        thaoTac.total = ret.case1;
                                        thuTuc.totalXuLy += ret.case1;
                                        $scope.$apply();
                                    });
                                }

                                else if (thaoTac.url.indexOf('/vanthuduyet') != -1) {
                                    _ret = {
                                        thuTuc: thuTuc.thuTucIdEnum,
                                        formId: app.FORM_ID.FORM_VAN_THU_DUYET,
                                        listFormCase: [1]//FORM_CASE_VAN_THU_DUYET
                                    }
                                    abp.services.app.thuTucCommon.getCountThuTucDashBoard(_ret).then(function (ret) {
                                        thaoTac.total = ret.total;
                                        thuTuc.totalXuLy += ret.total;
                                        $scope.$apply();
                                    });
                                }
                                else if (thaoTac.url.indexOf('/vanthuduyetcongvan') != -1) {
                                    _ret = {
                                        thuTuc: thuTuc.thuTucIdEnum,
                                        formId: app.FORM_ID.FORM_VAN_THU_DUYET_CONG_VAN
                                    };
                                    abp.services.app.thuTucCommon.getCountFormCaseThuTucSql(_ret).then(function (ret) {
                                        thaoTac.total = ret.case1;
                                        thuTuc.totalXuLy += ret.case1;
                                        $scope.$apply();
                                    });
                                }

                                else if (thaoTac.url.indexOf('/tonghopthamxet') != -1) {
                                    _ret = {
                                        thuTuc: thuTuc.thuTucIdEnum,
                                        formId: app.FORM_ID.FORM_TONG_HOP_THAM_XET_HO_SO
                                    }
                                    abp.services.app.thuTucCommon.getCountFormCaseThuTucSql(_ret).then(function (ret) {
                                        thaoTac.total = ret.case1;
                                        thuTuc.totalXuLy += ret.case1;
                                        $scope.$apply();
                                    });
                                }

                                else {
                                    //thaoTac.url = "tracuucommonhoso";
                                }
                            });
                        });
                    }

                    var initMainFun = () => {
                        vm.gourl = function (url, thutuc) {
                            app.sessionStorage.set("prm_thuTucIdEnum", thutuc.thuTucIdEnum);
                            abp.ui.setBusy();
                            let _thutuc = thutuc.thuTucIdEnum < 10 ? "0" + thutuc.thuTucIdEnum : thutuc.thuTucIdEnum + "";
                            //TT04
                            if (url.indexOf('/tonghopthamdinhcoso') != -1) {
                                $rootScope.tabMenu = {
                                    title: thutuc.maThuTuc + " - Tổng hợp thẩm định cơ sở",
                                    content: `<div app.directives.dirpages.tonghopthamdinh` + _thutuc + `></div>`,
                                    menulabel: 'tonghopthamdinhcoso' + thutuc.maThuTuc,
                                    currentThuTuc: thutuc.thuTucIdEnum,
                                    currentMaThuTuc: thutuc.maThuTuc,
                                };
                            }
                            //if (url.indexOf('/thamxethoso') != -1) {
                            //    $rootScope.tabMenu = {
                            //        title: thutuc.maThuTuc + " - Thẩm xét hồ sơ",
                            //        content: `<div app.directives.dirpages.thamxethoso` + _thutuc + `></div>`,
                            //        menulabel: 'thamxethoso' + thutuc.maThuTuc,
                            //        currentThuTuc: thutuc.thuTucIdEnum,
                            //        currentMaThuTuc: thutuc.maThuTuc,
                            //    };
                            //}
                            //dùng Chung
                            else if (url.indexOf('/dangkyhoso') > -1) {
                                $rootScope.tabMenu = {
                                    title: thutuc.maThuTuc + " - Đăng ký hồ sơ",
                                    content: `<app.directives.dirpages.dangkyhoso` + _thutuc + ` />`,
                                    menulabel: 'dangkyhoso_' + thutuc.maThuTuc,
                                    currentThuTuc: thutuc.thuTucIdEnum,
                                    currentMaThuTuc: thutuc.maThuTuc,
                                };
                            }
                            else if (url.indexOf('/rasoathoso') > -1) {
                                $rootScope.tabMenu = {
                                    title: thutuc.maThuTuc + " - Một cửa rà soát",
                                    content: `<div app.directives.dirpages.motcuarasoat` + _thutuc + `></div>`,
                                    menulabel: 'motcuarasoat_' + thutuc.maThuTuc,
                                };
                            }
                            else if (url.indexOf('/motcuaphancong') > -1) {
                                $rootScope.tabMenu = {
                                    title: thutuc.maThuTuc + " - Một cửa phân công",
                                    content: `<div app.directives.dirpages.motcuaphancong` + _thutuc + `></div>`,
                                    menulabel: 'motcuaphancong_' + thutuc.maThuTuc,
                                };
                            }
                            else if (url.indexOf('/phongbanphancong') > -1) {
                                $rootScope.tabMenu = {
                                    title: thutuc.maThuTuc + " - Phân công hồ sơ",
                                    content: `<div app.directives.dirpages.phongbanphancong` + _thutuc + `></div>`,
                                    menulabel: 'phongbanphancong_' + thutuc.maThuTuc,
                                    currentThuTuc: thutuc.thuTucIdEnum,
                                    currentMaThuTuc: thutuc.maThuTuc,
                                };
                            }
                            else if (url.indexOf('/thamxethoso') > -1) {
                                $rootScope.tabMenu = {
                                    title: thutuc.maThuTuc + " - Thẩm xét hồ sơ",
                                    content: `<div app.directives.dirpages.thamxethoso` + _thutuc + `></div>`,
                                    menulabel: 'thamxethoso' + thutuc.maThuTuc,
                                    currentThuTuc: thutuc.thuTucIdEnum,
                                    currentMaThuTuc: thutuc.maThuTuc,
                                };
                            }
                            else if (url.indexOf('/tonghopthamxet') > -1) {
                                $rootScope.tabMenu = {
                                    title: thutuc.maThuTuc + " - Tổng hợp thẩm xét",
                                    content: `<div app.directives.dirpages.tonghopthamxet` + _thutuc + `></div>`,
                                    menulabel: 'tonghopthamxet' + thutuc.maThuTuc,
                                    currentThuTuc: thutuc.thuTucIdEnum,
                                    currentMaThuTuc: thutuc.maThuTuc,
                                };
                            }
                            else if (url.indexOf('/tonghopthamdinh') > -1) {
                                $rootScope.tabMenu = {
                                    title: thutuc.maThuTuc + " - Tổng hợp thẩm định",
                                    content: `<div app.directives.dirpages.tonghopthamdinh` + _thutuc + `></div>`,
                                    menulabel: 'tonghopthamdinh' + thutuc.maThuTuc,
                                    currentThuTuc: thutuc.thuTucIdEnum,
                                    currentMaThuTuc: thutuc.maThuTuc,
                                };
                            }
                            else if (url.indexOf('/phophongduyet') > -1) {
                                $rootScope.tabMenu = {
                                    title: thutuc.maThuTuc + " - Duyệt thẩm xét",
                                    content: `<div app.directives.dirpages.phophongduyet` + _thutuc + `></div>`,
                                    menulabel: 'phophongduyet_' + thutuc.maThuTuc,
                                    currentThuTuc: thutuc.thuTucIdEnum,
                                    currentMaThuTuc: thutuc.maThuTuc,
                                };
                            }
                            else if (url.indexOf('/truongphongduyet') != -1) {
                                $rootScope.tabMenu = {
                                    title: thutuc.maThuTuc + " - Duyệt thẩm xét",
                                    content: `<div app.directives.dirpages.truongphongduyet` + _thutuc + `></div>`,
                                    menulabel: 'truongphongduyet_' + thutuc.maThuTuc,
                                    currentThuTuc: thutuc.thuTucIdEnum,
                                    currentMaThuTuc: thutuc.maThuTuc,
                                };
                            }
                            else if (url.indexOf('/thamdinhtruongphongduyet') != -1) {
                                $rootScope.tabMenu = {
                                    title: thutuc.maThuTuc + " - Duyệt thẩm định",
                                    content: `<div app.directives.dirpages.truongphongduyetthamdinh` + _thutuc + `></div>`,
                                    menulabel: 'thamdinhtruongphongduyet' + thutuc.maThuTuc,
                                    currentThuTuc: thutuc.thuTucIdEnum,
                                    currentMaThuTuc: thutuc.maThuTuc,
                                };
                            }

                            else if (url.indexOf('/lanhdaocucduyet') != -1) {
                                $rootScope.tabMenu = {
                                    title: thutuc.maThuTuc + " - Duyệt thẩm xét",
                                    content: `<div app.directives.dirpages.lanhdaocucduyet` + _thutuc + `></div>`,
                                    menulabel: 'lanhdaocucduyet_' + thutuc.maThuTuc,
                                    currentThuTuc: thutuc.thuTucIdEnum,
                                    currentMaThuTuc: thutuc.maThuTuc,
                                };
                            }
                            else if (url.indexOf('/thamdinhlanhdaocucduyet') != -1) {
                                $rootScope.tabMenu = {
                                    title: thutuc.maThuTuc + " - Duyệt thẩm định",
                                    content: `<div app.directives.dirpages.lanhdaocucduyetthamdinh` + _thutuc + `></div>`,
                                    menulabel: 'thamdinhlanhdaocucduyet_' + thutuc.maThuTuc,
                                    currentThuTuc: thutuc.thuTucIdEnum,
                                    currentMaThuTuc: thutuc.maThuTuc,
                                };
                            }

                            else if (url.indexOf('/congvanlanhdaocucduyet') != -1) {
                                $rootScope.tabMenu = {
                                    title: thutuc.maThuTuc + " - Duyệt công văn",
                                    content: `<div app.directives.dirpages.lanhdaocucduyetcongvan` + _thutuc + `></div>`,
                                    menulabel: 'lanhdaocucduyetcongvan_' + thutuc.maThuTuc,
                                    currentThuTuc: thutuc.thuTucIdEnum,
                                    currentMaThuTuc: thutuc.maThuTuc,
                                };
                            }
                            else if (url.indexOf('/lanhdaoboduyet') != -1) {
                                $rootScope.tabMenu = {
                                    title: thutuc.maThuTuc + " - Duyệt hồ sơ",
                                    content: `<div app.directives.dirpages.lanhdaoboduyet` + _thutuc + `></div>`,
                                    menulabel: 'lanhdaoboduyet_' + thutuc.maThuTuc,
                                    currentThuTuc: thutuc.thuTucIdEnum,
                                    currentMaThuTuc: thutuc.maThuTuc,
                                };
                            }
                            else if (url.indexOf('/vanthuduyet') != -1) {
                                $rootScope.tabMenu = {
                                    title: thutuc.maThuTuc + " - Văn thư duyệt",
                                    content: `<div app.directives.dirpages.vanthuduyet` + _thutuc + `></div>`,
                                    menulabel: 'vanthuduyet' + thutuc.maThuTuc,
                                    currentThuTuc: thutuc.thuTucIdEnum,
                                    currentMaThuTuc: thutuc.maThuTuc,
                                };
                            }
                            else if (url.indexOf('/congvanvanthuduyet') != -1) {
                                $rootScope.tabMenu = {
                                    title: thutuc.maThuTuc + " - Đóng dấu công văn",
                                    content: `<div app.directives.dirpages.vanthuduyetcongvan` + _thutuc + `></div>`,
                                    menulabel: 'congvanvanthuduyet' + thutuc.maThuTuc,
                                    currentThuTuc: thutuc.thuTucIdEnum,
                                    currentMaThuTuc: thutuc.maThuTuc,
                                };
                            }
                            else if (url.indexOf('/thamdinhvanthuduyet') != -1) {
                                $rootScope.tabMenu = {
                                    title: thutuc.maThuTuc + " - Đóng dấu thẩm định",
                                    content: `<div app.directives.dirpages.vanthuduyetthamdinh` + _thutuc + `></div>`,
                                    menulabel: 'thamdinhvanthuduyet' + thutuc.maThuTuc,
                                    currentThuTuc: thutuc.thuTucIdEnum,
                                    currentMaThuTuc: thutuc.maThuTuc,
                                };
                            }
                            else if (url.indexOf('/xacnhanthanhtoan') != -1) {
                                $rootScope.tabMenu = {
                                    title: thutuc.maThuTuc + " - Xác nhận thanh toán",
                                    content: `<div app.directives.dirpages.xacnhanthanhtoan` + _thutuc + `></div>`,
                                    menulabel: 'xacnhanthanhtoan' + thutuc.maThuTuc,
                                    currentThuTuc: thutuc.thuTucIdEnum,
                                    currentMaThuTuc: thutuc.maThuTuc,
                                };
                            }
                            else if (url.indexOf('/chuyengiathamdinh') != -1) {
                                $rootScope.tabMenu = {
                                    title: thutuc.maThuTuc + " - Chuyên gia thẩm định",
                                    content: `<div app.directives.dirpages.chuyengiathamdinh` + _thutuc + `></div>`,
                                    menulabel: 'chuyengiathamdinh' + thutuc.maThuTuc,
                                    currentThuTuc: thutuc.thuTucIdEnum,
                                    currentMaThuTuc: thutuc.maThuTuc,
                                };
                            }
                            else if (url.indexOf('/hoidongthamdinh') != -1) {
                                $rootScope.tabMenu = {
                                    title: thutuc.maThuTuc + " - Hội đồng thẩm định",
                                    content: `<div app.directives.dirpages.hoidongthamdinh` + _thutuc + `></div>`,
                                    menulabel: 'hoidongthamdinh' + thutuc.maThuTuc,
                                    currentThuTuc: thutuc.thuTucIdEnum,
                                    currentMaThuTuc: thutuc.maThuTuc,
                                };
                            }

                            else {
                                $state.go(url);
                            }
                        }

                        vm.filterthutuc = function () {
                            let _filter = vm.filterText.replace(/  /g, ' ').trim().toLowerCase();

                            vm.arrThuTuc = $filter('filter')(vm.thuTucs, function (item) {
                                return (_filter == '' || item.maThuTuc.toLowerCase().indexOf(_filter) > -1
                                    || item.tenKhongDau.toLowerCase().indexOf(_filter) > -1
                                    || item.tenThuTuc.toLowerCase().indexOf(_filter) > -1);
                            });
                        }

                        vm.loadThuTuc = function () {
                            abp.services.app.thuTuc.dataThuTucYeuThich().then(function (d) {
                                $scope.danhSachThuTucYeuThich = d;
                                vm.thuTucs = d;
                                vm.arrThuTuc = d;
                                console.log(vm.thuTucs, "vm.thuTucs");
                                getRequestTotalThuTuc();
                            });
                        }
                        vm.tongHoSoCanXuLy = function () {
                            let _total = 0;
                            angular.forEach(vm.arrThuTuc, function (tt) {
                                _total += tt.totalXuLy;
                            });
                            return _total;
                        }
                    }

                    var init = () => {
                        initVar();
                        initMainFun();
                        if (vm.roleLv || vm.isAdmin == true) {
                            vm.loadThuTuc();
                        }
                    }
                    init();
                }]
            return {
                restrict: 'EA',
                scope: {
                    //functionRefesh: '&'
                },
                controller: controller,
                controllerAs: 'vm',
                bindToController: false,
                link: function (scope, elem, attr, ctrl) {
                    $templateRequest("/App/_directives/dirDanhSachThuTucDashBoard/dirDanhSachThuTucDashBoard.cshtml").then(function (html) {
                        var template = angular.element(html);
                        elem.append(template);
                        $compile(template)(scope);
                    });
                    //scope.functionRefesh({ theDirFn: scope.refresh });
                }
            };
        }
    ]);
})();