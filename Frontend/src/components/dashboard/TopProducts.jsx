import Icon from '../common/Icon'
import { products } from '../../data/dashboardData'

export default function TopProducts() {
  return (
    <article className="card products-card">
      <div className="card-header compact">
        <h3>Productos Más Solicitados</h3>
        <button type="button">
          Ver más <Icon name="chevron" />
        </button>
      </div>
      <div className="products-list">
        {products.map(({ name, value, icon }, index) => (
          <div key={name} className="product-row">
            <div className="product-rank">{index + 1}</div>
            <div className="product-name-wrap">
              <div className="product-icon"><Icon name={icon} /></div>
              <div className="product-name">{name}</div>
            </div>
            <div className="product-bar">
              <span style={{ width: `${Math.min(value / 4, 100)}%` }} />
            </div>
            <div className="product-value">{value} ventas</div>
          </div>
        ))}
      </div>
    </article>
  )
}
