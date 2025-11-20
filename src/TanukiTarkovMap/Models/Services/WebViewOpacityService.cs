using Microsoft.Web.WebView2.Wpf;
using System;
using System.Threading.Tasks;
using TanukiTarkovMap.Models.Utils;

namespace TanukiTarkovMap.Models.Services
{
    /// <summary>
    /// WebView2 투명도 관리 서비스
    /// </summary>
    public static class WebViewOpacityService
    {
        /// <summary>
        /// WebView2 투명도 초기화를 위한 환경 변수 설정
        /// CoreWebView2 생성 전에 호출되어야 함
        /// </summary>
        public static void InitializeEnvironment()
        {
            // WebView2 투명도를 위한 환경 변수 설정
            Environment.SetEnvironmentVariable("WEBVIEW2_DEFAULT_BACKGROUND_COLOR", "00FFFFFF");
            Logger.SimpleLog("[WebViewOpacityService] Environment variable set for transparency");
        }

        /// <summary>
        /// WebView2의 배경을 투명하게 설정
        /// </summary>
        /// <param name="webView">투명하게 만들 WebView2 인스턴스</param>
        public static void SetWebViewTransparent(WebView2 webView)
        {
            if (webView == null)
            {
                Logger.Error("[WebViewOpacityService] WebView2 is null", null);
                return;
            }

            // WebView2 컨트롤의 배경색을 투명으로 설정
            webView.DefaultBackgroundColor = System.Drawing.Color.Transparent;
            Logger.SimpleLog("[WebViewOpacityService] WebView2 background set to transparent");
        }

        /// <summary>
        /// 웹 페이지의 배경을 투명하게 설정
        /// CoreWebView2가 초기화된 후에 호출되어야 함
        /// </summary>
        /// <param name="webView">대상 WebView2 인스턴스</param>
        public static async Task SetWebPageTransparentAsync(WebView2 webView)
        {
            if (webView?.CoreWebView2 == null)
            {
                Logger.Error("[WebViewOpacityService] CoreWebView2 is not initialized", null);
                return;
            }

            try
            {
                // CSS 강제 주입 방식: 주요 요소들을 선택적으로 투명화
                await webView.CoreWebView2.ExecuteScriptAsync(@"
                    (function() {
                        // 기본 요소 투명화
                        document.body.style.backgroundColor = 'transparent';
                        document.documentElement.style.backgroundColor = 'transparent';

                        // CSS 스타일 주입
                        const styleId = 'webview-transparency';
                        let style = document.getElementById(styleId);
                        if (!style) {
                            style = document.createElement('style');
                            style.id = styleId;
                            document.head.appendChild(style);
                        }

                        // 선택적 투명화: 주요 컨테이너와 배경 요소만 타겟팅
                        style.textContent = `
                            body, html {
                                background-color: transparent !important;
                                background-image: none !important;
                            }
                            #app, .page-content, .map-cont, .bg-grid {
                                background-color: transparent !important;
                                background-image: none !important;
                            }
                            svg {
                                background-color: transparent !important;
                            }
                        `;

                        console.log('[WebViewOpacity] Transparency applied');
                    })();
                ");
                Logger.SimpleLog("[WebViewOpacityService] Web page background set to transparent");
            }
            catch (Exception ex)
            {
                Logger.Error("[WebViewOpacityService] Failed to set web page background to transparent", ex);
            }
        }

        /// <summary>
        /// WebView2를 완전히 투명하게 설정 (컨트롤 + 웹 페이지)
        /// </summary>
        /// <param name="webView">대상 WebView2 인스턴스</param>
        public static async Task SetFullTransparencyAsync(WebView2 webView)
        {
            if (webView == null)
            {
                Logger.Error("[WebViewOpacityService] WebView2 is null", null);
                return;
            }

            // WebView2 컨트롤 배경 투명화
            SetWebViewTransparent(webView);

            // 웹 페이지 배경 투명화 (CoreWebView2가 초기화되어 있는 경우)
            if (webView.CoreWebView2 != null)
            {
                await SetWebPageTransparentAsync(webView);
            }
        }
    }
}
