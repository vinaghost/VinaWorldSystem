import { Fragment, useEffect, useMemo, useState } from 'react'
import { stable_change, troop_cost, warehouse_change } from './data/troop'
import PWABadge from './PWABadge.jsx'
import { TABS } from './tabs'
import './App.css'

const INITIALS = {
  balance: 3000,
  exp: 0,
  cp: 0,
  population: 0,
}

function getNumber(value) {
  return Number(value) || 0
}

function formatNumber(value) {
  return getNumber(value).toLocaleString()
}

function formatRounded(value) {
  return Math.round(getNumber(value)).toLocaleString()
}

function buildComputedRows(rows, options = {}) {
  const {
    calcInsertIndex = -1,
    secondCalcInsertIndex = -1,
    researchCost = 0,
    trainCost = 0,
    secondTrainCost = 0,
    enableCalcRows = false,
    applyEquitesOverrides = false,
    stableOverrideIndex = -1,
    warehouseUpdateIndex = -1,
  } = options
  let balance = INITIALS.balance
  let previousRewardRes = 0
  let exp = INITIALS.exp
  let cp = INITIALS.cp
  let population = INITIALS.population

  const applyComputedValues = (rowData) => {
    balance += previousRewardRes - getNumber(rowData.cost)
    const displayBalance = balance

    previousRewardRes = getNumber(rowData['reward res'])
    exp += getNumber(rowData['reward exp'])
    cp += getNumber(rowData['cp prod'])
    population += getNumber(rowData.pop)

    return {
      ...rowData,
      balance: displayBalance,
      exp,
      cp,
      population,
    }
  }

  const result = []

  rows.forEach((row, index) => {
    let currentRow = row

    if (applyEquitesOverrides && index === stableOverrideIndex) {
      currentRow = {
        ...row,
        task: stable_change[1].task,
        cost: stable_change[1].cost,
        'cp prod': stable_change[1]['cp prod'],
        pop: stable_change[1].pop,
      }
    }

    if (applyEquitesOverrides && index === warehouseUpdateIndex) {
      currentRow = {
        ...row,
        task: warehouse_change[1].task,
        cost: warehouse_change[1].cost,
        'cp prod': warehouse_change[1]['cp prod'],
        pop: warehouse_change[1].pop,
      }
    }

    if (enableCalcRows && index === calcInsertIndex) {
      if (applyEquitesOverrides) {
        result.push(
          applyComputedValues({
            kind: 'task',
            checkKey: 'equites-warehouse-to-5',
            displayNo: '',
            'To do': 'Warehouse',
            Tier: 0,
            task: warehouse_change[0].task,
            cost: warehouse_change[0].cost,
            'reward res': 0,
            'reward exp': 0,
            'cp prod': warehouse_change[0]['cp prod'],
            pop: warehouse_change[0].pop,
          }),
        )
      }

      result.push(
        applyComputedValues({
          kind: 'calc-research',
          cost: researchCost,
          'reward res': 0,
          'reward exp': 0,
          'cp prod': 0,
          pop: 0,
        }),
      )

      result.push(
        applyComputedValues({
          kind: 'calc-train',
          cost: trainCost,
          'reward res': 0,
          'reward exp': 0,
          'cp prod': 0,
          pop: 0,
        }),
      )
    }

    if (enableCalcRows && index === secondCalcInsertIndex) {
      result.push(
        applyComputedValues({
          kind: 'calc-train-2',
          cost: secondTrainCost,
          'reward res': 0,
          'reward exp': 0,
          'cp prod': 0,
          pop: 0,
        }),
      )
    }

    result.push(
      applyComputedValues({
        kind: 'task',
        checkKey: String(index),
        index,
        ...currentRow,
      }),
    )
  })

  return result
}

function App() {
  const [activeTab, setActiveTab] = useState(TABS[0].id)
  const [checkedByTab, setCheckedByTab] = useState({})
  const [selectedUnit, setSelectedUnit] = useState(troop_cost[0]?.unit ?? '')
  const [farmUnitCount, setFarmUnitCount] = useState(10)
  const [farmUnitCountSecond, setFarmUnitCountSecond] = useState(10)

  useEffect(() => {
    const loaded = {}

    TABS.forEach((tab) => {
      try {
        const saved = window.localStorage.getItem(`spawn-checklist:${tab.id}`)
        if (!saved) {
          loaded[tab.id] = {}
          return
        }

        const parsed = JSON.parse(saved)
        loaded[tab.id] = parsed && typeof parsed === 'object' ? parsed : {}
      } catch {
        loaded[tab.id] = {}
      }
    })

    setCheckedByTab(loaded)
  }, [])

  const activeTabConfig = TABS.find((tab) => tab.id === activeTab) ?? TABS[0]
  const notesByIndex = useMemo(() => {
    const grouped = {}

    ;(activeTabConfig.notes ?? []).forEach((noteItem) => {
      const noteIndex = Number(noteItem.index)
      if (Number.isNaN(noteIndex) || !noteItem.note) {
        return
      }

      if (!grouped[noteIndex]) {
        grouped[noteIndex] = []
      }

      grouped[noteIndex].push(noteItem.note)
    })

    return grouped
  }, [activeTabConfig])

  const activeChecks = checkedByTab[activeTab] ?? {}
  const selectedTroop = useMemo(
    () => troop_cost.find((troop) => troop.unit === selectedUnit) ?? troop_cost[0],
    [selectedUnit],
  )
  const activeFarmConfig = activeTabConfig.farmConfig

  const researchCost = getNumber(selectedTroop?.research)
  const trainCost = getNumber(farmUnitCount) * getNumber(selectedTroop?.train)
  const secondTrainCost = getNumber(farmUnitCountSecond) * getNumber(selectedTroop?.train)
  const breakEven = researchCost + trainCost
  const secondBreakEven = secondTrainCost
  const fullRaids = getNumber(selectedTroop?.capacity)
    ? breakEven / getNumber(selectedTroop.capacity)
    : 0
  const secondFullRaids = getNumber(selectedTroop?.capacity)
    ? secondBreakEven / getNumber(selectedTroop.capacity)
    : 0
  const perHour24 = breakEven / 24
  const perHour48 = breakEven / 48
  const secondPerHour24 = secondBreakEven / 24
  const secondPerHour48 = secondBreakEven / 48
  const showFarmCalc = Boolean(activeFarmConfig)
  const applyEquitesOverrides = showFarmCalc && selectedTroop?.unit === 'Equites Imperatoris'
  const computedRows = useMemo(
    () =>
      buildComputedRows(activeTabConfig.rows, {
        calcInsertIndex: activeFarmConfig?.firstInsertIndex ?? -1,
        secondCalcInsertIndex: activeFarmConfig?.secondInsertIndex ?? -1,
        researchCost,
        trainCost,
        secondTrainCost,
        enableCalcRows: showFarmCalc,
        applyEquitesOverrides,
        stableOverrideIndex: activeFarmConfig?.equitesStableIndex ?? -1,
        warehouseUpdateIndex: activeFarmConfig?.equitesWarehouseUpdateIndex ?? -1,
      }),
    [
      activeTabConfig,
      activeFarmConfig,
      researchCost,
      trainCost,
      secondTrainCost,
      showFarmCalc,
      applyEquitesOverrides,
    ],
  )

  const toggleDone = (rowKey) => {
    setCheckedByTab((previous) => {
      const currentTabChecks = previous[activeTab] ?? {}
      const nextTabChecks = {
        ...currentTabChecks,
        [rowKey]: !currentTabChecks[rowKey],
      }

      const next = {
        ...previous,
        [activeTab]: nextTabChecks,
      }

      window.localStorage.setItem(
        `spawn-checklist:${activeTab}`,
        JSON.stringify(nextTabChecks),
      )

      return next
    })
  }

  return (
    <main className="app-shell">
      <header className="app-header">
        <h1>Spawn Village Development Checklist</h1>
        <p>Track your build order for 3-party and 4-party routes.</p>
      </header>

      <nav className="tabs" aria-label="Checklist tabs">
        {TABS.map((tab) => (
          <button
            key={tab.id}
            type="button"
            className={`tab-btn ${tab.id === activeTab ? 'active' : ''}`}
            onClick={() => setActiveTab(tab.id)}
          >
            {tab.label}
          </button>
        ))}
      </nav>

      <section className="table-wrap">
        <table>
          <thead>
            <tr>
              <th>No.</th>
              <th>Done</th>
              <th>To do</th>
              <th>Task</th>
              <th>cost</th>
              <th>reward res</th>
              <th>reward exp</th>
              <th>cp prod</th>
              <th>pop</th>
              <th>balance</th>
              <th>exp</th>
              <th>cp</th>
              <th>population</th>
            </tr>
          </thead>
          <tbody>
            {computedRows.map((row) => {
              if (row.kind === 'calc-research') {
                return (
                  <tr key={`${activeTab}-calc-research`} className="farm-calc-row">
                    <td></td>
                    <td></td>
                    <td className="farm-calc-label">Research</td>
                    <td>
                      <select
                        value={selectedUnit}
                        onChange={(event) => setSelectedUnit(event.target.value)}
                        className="farm-calc-input farm-calc-unit-input"
                        aria-label="Select troop unit"
                      >
                        {troop_cost.map((troop) => (
                          <option key={troop.unit} value={troop.unit}>
                            {troop.unit}
                          </option>
                        ))}
                      </select>
                    </td>
                    <td className="farm-calc-value">{formatNumber(researchCost)}</td>
                    <td className="farm-calc-label">Break even</td>
                    <td className="farm-calc-label">Full raids</td>
                    <td className="farm-calc-label">Per hour (24h)</td>
                    <td className="farm-calc-label">Per hour (48h)</td>
                    <td className="farm-calc-value">{formatNumber(row.balance)}</td>
                    <td className="farm-calc-value">{formatNumber(row.exp)}</td>
                    <td className="farm-calc-value">{formatNumber(row.cp)}</td>
                    <td className="farm-calc-value">{formatNumber(row.population)}</td>
                  </tr>
                )
              }

              if (row.kind === 'calc-train') {
                return (
                  <tr key={`${activeTab}-calc-train`} className="farm-calc-row">
                    <td></td>
                    <td></td>
                    <td className="farm-calc-label">Farm units</td>
                    <td>
                      <div className="farm-calc-control">
                        <input
                          type="number"
                          min="0"
                          value={farmUnitCount}
                          onChange={(event) => setFarmUnitCount(getNumber(event.target.value))}
                          className="farm-calc-input"
                          aria-label="Farm units to make"
                        />
                        <span className="farm-calc-suffix">to make</span>
                      </div>
                    </td>
                    <td className="farm-calc-value">{formatNumber(trainCost)}</td>
                    <td className="farm-calc-value">{formatNumber(breakEven)}</td>
                    <td className="farm-calc-value">{formatRounded(fullRaids)}</td>
                    <td className="farm-calc-value">{formatRounded(perHour24)}</td>
                    <td className="farm-calc-value">{formatRounded(perHour48)}</td>
                    <td className="farm-calc-value">{formatNumber(row.balance)}</td>
                    <td className="farm-calc-value">{formatNumber(row.exp)}</td>
                    <td className="farm-calc-value">{formatNumber(row.cp)}</td>
                    <td className="farm-calc-value">{formatNumber(row.population)}</td>
                  </tr>
                )
              }

              if (row.kind === 'calc-train-2') {
                return (
                  <tr key={`${activeTab}-calc-train-2`} className="farm-calc-row">
                    <td></td>
                    <td></td>
                    <td className="farm-calc-label">Farm units</td>
                    <td>
                      <div className="farm-calc-control">
                        <input
                          type="number"
                          min="0"
                          value={farmUnitCountSecond}
                          onChange={(event) => setFarmUnitCountSecond(getNumber(event.target.value))}
                          className="farm-calc-input"
                          aria-label="Farm units to make second"
                        />
                        <span className="farm-calc-suffix">to make</span>
                      </div>
                    </td>
                    <td className="farm-calc-value">{formatNumber(secondTrainCost)}</td>
                    <td className="farm-calc-value">{formatNumber(secondBreakEven)}</td>
                    <td className="farm-calc-value">{formatRounded(secondFullRaids)}</td>
                    <td className="farm-calc-value">{formatRounded(secondPerHour24)}</td>
                    <td className="farm-calc-value">{formatRounded(secondPerHour48)}</td>
                    <td className="farm-calc-value">{formatNumber(row.balance)}</td>
                    <td className="farm-calc-value">{formatNumber(row.exp)}</td>
                    <td className="farm-calc-value">{formatNumber(row.cp)}</td>
                    <td className="farm-calc-value">{formatNumber(row.population)}</td>
                  </tr>
                )
              }

              const rowCheckKey = row.checkKey ?? String(row.index)
              const isDone = Boolean(activeChecks[rowCheckKey])
              const rowNotes = notesByIndex[row.index] ?? []
              const rowNumberLabel = row.displayNo ?? row.index + 1
              const rowKeyBase = row.index ?? rowCheckKey

              return (
                <Fragment key={`${activeTab}-${rowKeyBase}-group`}>
                  {rowNotes.map((note, noteIndex) => (
                    <tr key={`${activeTab}-${rowKeyBase}-note-${noteIndex}`} className="note-row">
                      <td colSpan={13}>{note}</td>
                    </tr>
                  ))}
                  <tr key={`${activeTab}-${rowKeyBase}`} className={isDone ? 'done-row' : ''}>
                    <td>{rowNumberLabel}</td>
                    <td>
                      <input
                        type="checkbox"
                        checked={isDone}
                        onChange={() => toggleDone(rowCheckKey)}
                        aria-label={`Mark row ${rowNumberLabel || rowCheckKey} as done`}
                      />
                    </td>
                    <td>{row['To do']}</td>
                    <td>{row.task}</td>
                    <td>{formatNumber(row.cost)}</td>
                    <td>{formatNumber(row['reward res'])}</td>
                    <td>{formatNumber(row['reward exp'])}</td>
                    <td>{row['cp prod']}</td>
                    <td>{row.pop}</td>
                    <td>{formatNumber(row.balance)}</td>
                    <td>{row.exp}</td>
                    <td>{row.cp}</td>
                    <td>{row.population}</td>
                  </tr>
                </Fragment>
              )
            })}
          </tbody>
        </table>
      </section>

      <div className="pwa-badge-wrap">
        <PWABadge />
      </div>
    </main>
  )
}

export default App
