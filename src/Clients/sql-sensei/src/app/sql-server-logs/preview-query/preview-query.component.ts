import { Component, Input, OnDestroy, OnInit, OnChanges, SimpleChanges } from '@angular/core';
import { QueryPlanResponse, ServersApiService, SqlServerBadQuery } from '../servers-api.service';
import { Subject } from 'rxjs';
import { takeUntil, switchMap } from 'rxjs/operators';
import { format } from 'sql-formatter';

declare var QP: any;

@Component({
  selector: 'app-preview-query',
  templateUrl: './preview-query.component.html',
  styleUrls: ['./preview-query.component.css'],
})
export class PreviewQueryComponent implements OnInit, OnChanges, OnDestroy {
  @Input() serverId?: number;
  @Input() query?: SqlServerBadQuery;

  private ngUnsubscribe: Subject<void> = new Subject<void>();
  plan?: QueryPlanResponse;

  constructor(private api: ServersApiService) {}

  ngOnInit(): void {
    this.listenToQueryIdChanges();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['query'] && !changes['query'].firstChange) {
      this.listenToQueryIdChanges();
    }
  }

  ngOnDestroy(): void {
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }

  private listenToQueryIdChanges(): void {
    const queryIdChanges$ = new Subject<number>();

    queryIdChanges$
      .pipe(
        switchMap((queryId: number) => this.api.getQueryPlan(this.serverId, queryId)),
        takeUntil(this.ngUnsubscribe)
      )
      .subscribe((plan) => {
        this.plan = plan;
        this.plan.sqlText = format(this.plan.sqlText, { language: 'transactsql', keywordCase: 'upper', indentStyle: 'tabularLeft' });

        QP.showPlan(document.getElementById('query'), this.plan.xmlPlan, { jsTooltips: false });
      });

    if (this.query?.id) {
      queryIdChanges$.next(this.query.id);
    }
  }
}
