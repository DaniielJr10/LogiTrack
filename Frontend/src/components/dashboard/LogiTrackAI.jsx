export default function LogiTrackAI() {
  return (
    <article className="card ai-card">
      <div className="ai-header-row">
        <span className="ai-spark">✦</span>
        <h3>LogiTrack AI</h3>
      </div>

      <p>Pregunta, analiza y optimiza tu logística con inteligencia artificial.</p>

      <button type="button">Abrir Asistente <span aria-hidden="true">→</span></button>

      <div className="ai-visual" aria-hidden="true">
        <div className="robot">
          <div className="head">
            <div className="antenna" />
            <div className="eyes"><span /><span /></div>
            <div className="mouth" />
          </div>
          <div className="arm arm-left" />
          <div className="arm arm-right" />
          <div className="body" />
          <div className="badge">AI</div>
        </div>
      </div>
    </article>
  )
}
