import { useEffect, useState } from 'react'
import FourPartySimTab from './Tabs/FourPartySimTab'
import FourPartyFarmTab from './Tabs/FourPartyFarmTab'
import ThreePartySimTab from './Tabs/ThreePartySimTab'
import ThreePartyFarmTab from './Tabs/ThreePartyFarmTab'
import { ChecklistTable } from './Tabs/ChecklistTabLayout'
import { CHECKLIST_DATA, COMMON_TRAILING_DATA } from './data/checklists'
import './App.css'

const TAB_ITEMS = [
  { name: '4 Party - Sim', Component: FourPartySimTab },
  { name: '4 Party - Farm', Component: FourPartyFarmTab },
  { name: '3 Party - Sim', Component: ThreePartySimTab },
  { name: '3 Party - Farm', Component: ThreePartyFarmTab },
]

const STORAGE_KEY = 'spawn-village-checklist-state-v1'

const createEmptyRow = () => ({
  id: crypto.randomUUID(),
  note: '',
  done: false,
  content: '',
  task: '',
  cost: '',
  rewardRes: '',
  rewardXp: '',
  cpProd: '',
  pop: '',
  balance: '',
  exp: '',
  cp: '',
  popTotal: '',
})

function App() {
  const [tribe, setTribe] = useState('gaul')
  const [heroLevel, setHeroLevel] = useState('0')
  const defaultActiveTab = TAB_ITEMS[0].name
  const defaultTabRows = TAB_ITEMS.reduce((acc, tab) => {
    const sheetRows = CHECKLIST_DATA[tab.name] || []
    acc[tab.name] = sheetRows.length > 0 ? sheetRows : [createEmptyRow()]
    return acc
  }, {})

  const defaultTrailingRows = COMMON_TRAILING_DATA
    .filter((row) => row.content && row.content.trim() !== '')
    .map((row) => ({ ...row, note: '' }))

  const getInitialState = () => {
    try {
      const raw = localStorage.getItem(STORAGE_KEY)
      if (!raw) {
        return {
          activeTab: defaultActiveTab,
          tabRows: defaultTabRows,
          trailingRows: defaultTrailingRows,
        }
      }

      const saved = JSON.parse(raw)
      const tabNames = new Set(TAB_ITEMS.map((tab) => tab.name))
      const savedActiveTab = typeof saved?.activeTab === 'string' ? saved.activeTab : ''
      const mergedActiveTab = tabNames.has(savedActiveTab) ? savedActiveTab : defaultActiveTab

      const mergedTabRows = TAB_ITEMS.reduce((acc, tab) => {
        const defaults = defaultTabRows[tab.name] || []
        const savedRows = Array.isArray(saved?.tabRows?.[tab.name]) ? saved.tabRows[tab.name] : []
        const savedDoneById = new Map(savedRows.map((row) => [row.id, Boolean(row.done)]))

        acc[tab.name] = defaults.map((row) => ({
          ...row,
          done: savedDoneById.has(row.id) ? savedDoneById.get(row.id) : row.done,
        }))

        return acc
      }, {})

      const savedTrailingRows = Array.isArray(saved?.trailingRows) ? saved.trailingRows : []
      const savedTrailingDoneById = new Map(savedTrailingRows.map((row) => [row.id, Boolean(row.done)]))
      const mergedTrailingRows = defaultTrailingRows.map((row) => ({
        ...row,
        done: savedTrailingDoneById.has(row.id) ? savedTrailingDoneById.get(row.id) : row.done,
      }))

      return {
        activeTab: mergedActiveTab,
        tabRows: mergedTabRows,
        trailingRows: mergedTrailingRows,
      }
    } catch {
      return {
        activeTab: defaultActiveTab,
        tabRows: defaultTabRows,
        trailingRows: defaultTrailingRows,
      }
    }
  }

  const [initialState] = useState(() => getInitialState())
  const [activeTab, setActiveTab] = useState(() => initialState.activeTab)
  const [tabRows, setTabRows] = useState(() =>
    initialState.tabRows,
  )

  const [trailingRows, setTrailingRows] = useState(() =>
    initialState.trailingRows,
  )
  const trailingNotes = COMMON_TRAILING_DATA.map((row) => row.note).filter(Boolean)

  useEffect(() => {
    const payload = {
      activeTab,
      tabRows,
      trailingRows,
    }

    localStorage.setItem(STORAGE_KEY, JSON.stringify(payload))
  }, [activeTab, tabRows, trailingRows])

  const updateRow = (tabName, rowId, updates) => {
    setTabRows((prev) => ({
      ...prev,
      [tabName]: prev[tabName].map((row) =>
        row.id === rowId ? { ...row, ...updates } : row,
      ),
    }))
  }

  const updateTrailingRow = (rowId, updates) => {
    setTrailingRows((prev) =>
      prev.map((row) =>
        row.id === rowId ? { ...row, ...updates } : row,
      ),
    )
  }

  const activeTabConfig = TAB_ITEMS.find((tab) => tab.name === activeTab)
  const ActiveTabComponent = activeTabConfig?.Component

  return (
    <main className="app-shell">
      <header className="app-header">
        <h1>Spawn Village Checklist</h1>
        <p></p>

        <div className="shared-controls" aria-label="Shared setup">
          <label className="field-group" htmlFor="tribe-select">
            <span>Tribe</span>
            <select
              id="tribe-select"
              value={tribe}
              onChange={(event) => setTribe(event.target.value)}
            >
              <option value="gaul">Gaul</option>
              <option value="roman">Roman</option>
              <option value="teuon">Teuon</option>
            </select>
          </label>

          <label className="field-group" htmlFor="hero-level-input">
            <span>Hero Level</span>
            <input
              id="hero-level-input"
              type="text"
              value={heroLevel}
              onChange={(event) => setHeroLevel(event.target.value)}
              placeholder="Enter hero level"
            />
          </label>
        </div>
      </header>

      <nav className="tabs" aria-label="Checklist tabs">
        {TAB_ITEMS.map((tab) => (
          <button
            type="button"
            key={tab.name}
            className={`tab-button ${activeTab === tab.name ? 'active' : ''}`}
            onClick={() => setActiveTab(tab.name)}
          >
            {tab.name}
          </button>
        ))}
      </nav>

      {ActiveTabComponent ? (
        <ActiveTabComponent
          tribe={tribe}
          heroLevel={heroLevel}
          rows={tabRows[activeTab]}
          onUpdateRow={(rowId, updates) => updateRow(activeTab, rowId, updates)}
        />
      ) : null}

      <section className="tab-panel more-cp-section" aria-live="polite">
        <div className="panel-topbar">
          <div className="tab-meta">
            <h2>More CP buildings</h2>
            {trailingNotes.length > 0
              ? trailingNotes.map((note, idx) => <p key={idx}>{note}</p>)
              : <p>There is error </p>}
          </div>
        </div>

        <ChecklistTable
          rows={trailingRows}
          onUpdateRow={(rowId, updates) => updateTrailingRow(rowId, updates)}
          hideNoteColumn
        />
      </section>
    </main>
  )
}

export default App
