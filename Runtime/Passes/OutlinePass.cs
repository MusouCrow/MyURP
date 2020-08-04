namespace UnityEngine.Rendering.Universal.Internal {
    public class OutlinePass : ScriptableRenderPass {
        private const string m_ProfilerTag = "Draw Outline";

        private FilteringSettings m_FilteringSettings;
        private ProfilingSampler m_ProfilingSampler;
        private ShaderTagId m_ShaderTagId;

        public OutlinePass(RenderPassEvent evt, RenderQueueRange renderQueueRange, LayerMask layerMask) {
            this.renderPassEvent = evt;
            this.m_FilteringSettings = new FilteringSettings(renderQueueRange, layerMask);
            this.m_ShaderTagId = new ShaderTagId("Outline");
            this.m_ProfilingSampler = new ProfilingSampler(m_ProfilerTag);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
            CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);

            using (new ProfilingScope(cmd, this.m_ProfilingSampler)) {
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                var drawSettings = CreateDrawingSettings(this.m_ShaderTagId, ref renderingData, renderingData.cameraData.defaultOpaqueSortFlags);
                context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref this.m_FilteringSettings);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}