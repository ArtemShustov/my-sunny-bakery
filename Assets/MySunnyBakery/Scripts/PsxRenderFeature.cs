using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

namespace MySunnyBakery {
    public class PsxRenderFeature : ScriptableRendererFeature {
        private static readonly int _resolution = Shader.PropertyToID("_Resolution");

        [Serializable]
        public class FeatureSettings {
            public int Height = 240;
            public RenderPassEvent RenderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
        }

        public FeatureSettings Settings = new FeatureSettings();
        private Pass _scriptablePass;
        private Material _material;

        public override void Create() {
            _material = CoreUtils.CreateEngineMaterial("Hidden/PSX_Screen");
            _scriptablePass = new Pass(Settings, _material);
        }

        protected override void Dispose(bool disposing) {
            CoreUtils.Destroy(_material);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
            if (_material == null) {
	            return;
            }
            if (renderingData.cameraData.cameraType is not (CameraType.Game or CameraType.SceneView)) {
	            return;
            }
            renderer.EnqueuePass(_scriptablePass);
        }

        private class Pass : ScriptableRenderPass {
            private FeatureSettings _settings;
            private Material _material;

            public Pass(FeatureSettings settings, Material material) {
                _settings = settings;
                _material = material;
                renderPassEvent = settings.RenderPassEvent;
            }

            private class PassData {
                public TextureHandle Source;
                public TextureHandle Temp;
                public Material Material;
                public Vector4 Resolution;
            }

            public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData) {
                var resourceData = frameData.Get<UniversalResourceData>();
                var cameraData = frameData.Get<UniversalCameraData>();

                var desc = cameraData.cameraTargetDescriptor;
                desc.depthBufferBits = 0;

                var aspectRatio = (float)desc.width / desc.height;
                var resolution = new Vector4(Mathf.Round(_settings.Height * aspectRatio), _settings.Height, 0, 0);

                var tempTexture = UniversalRenderer.CreateRenderGraphTexture(renderGraph, desc, "_TempColorTex", false);

                using (var builder = renderGraph.AddRasterRenderPass<PassData>("PSX Screen Downsample", out var passData)) {
                    passData.Source = resourceData.activeColorTexture;
                    passData.Material = _material;
                    passData.Resolution = resolution;

                    builder.UseTexture(passData.Source, AccessFlags.Read);
                    builder.SetRenderAttachment(tempTexture, 0, AccessFlags.Write);

                    builder.SetRenderFunc((PassData data, RasterGraphContext ctx) => {
                        data.Material.SetVector(_resolution, data.Resolution);
                        Blitter.BlitTexture(ctx.cmd, data.Source, new Vector4(1, 1, 0, 0), data.Material, 0);
                    });
                }

                using (var builder = renderGraph.AddRasterRenderPass<PassData>("PSX Screen Upsample", out var passData)) {
                    passData.Temp = tempTexture;

                    builder.UseTexture(passData.Temp, AccessFlags.Read);
                    builder.SetRenderAttachment(resourceData.activeColorTexture, 0, AccessFlags.Write);

                    builder.SetRenderFunc((PassData data, RasterGraphContext ctx) => {
                        Blitter.BlitTexture(ctx.cmd, data.Temp, new Vector4(1, 1, 0, 0), 0, false);
                    });
                }
            }
        }
    }
}
